using ApecCoreIdentity;
using AutoMapper;
using System.Security.Claims;
using UserServices.Application.ModelsDTO;
using UserServices.Domain.Models;
using ApecMovieCore.CoreHasing;
using UserServices.Domain.Interfaces;
using ApecMovieCore.BaseResponse;

namespace UserServices.Application.BussinessServices
{
    public class UserServiceImplementation : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly JwtTokenService _jwtTokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly BlacklistTokenService _blacklistTokenService;
        public UserServiceImplementation(IUserRepository userRepository, IMapper mapper, IRefreshTokenRepository refreshTokenRepository, BlacklistTokenService blacklistTokenService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _jwtTokenService = new JwtTokenService();
            _refreshTokenRepository = refreshTokenRepository;
            _blacklistTokenService = blacklistTokenService;
        }

        public async Task<UserDTO> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<IEnumerable<UserDTO>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<UserDTO> AddAsync(UserDTO userDTO)
        {
            var user = _mapper.Map<User>(userDTO);
            user.Id = Guid.NewGuid();
            user.Status = true;
            user.Role = "User";
            // Mã hóa mật khẩu trước khi lưu vào cơ sở dữ liệu
            user.Password = Encrypt.HashingCore(user.Password);
            await _userRepository.AddAsync(user);
            return userDTO;
        }

        public async Task UpdateAsync(Guid id, UserDTO userDTO)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                throw new Exception("User not found");
            }

            // Update only the changed fields
            existingUser.Name = userDTO.Name ?? existingUser.Name;
            existingUser.Email = userDTO.Email ?? existingUser.Email;
            if (!string.IsNullOrEmpty(userDTO.Password))
            {
                existingUser.Password = Encrypt.HashingCore(userDTO.Password);
            }

            await _userRepository.UpdateAsync(existingUser);
        }


        public async Task DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            await _userRepository.DeleteAsync(user);
        }

        public async Task<Response<LoginResponse>> LoginAsync(LoginRequest loginModel)
        {
            try
            {
                var user = await _userRepository.GetByNameAsync(loginModel.Name);

                if (user == null || user.Password != Encrypt.HashingCore(loginModel.Password))
                {
                    throw new UnauthorizedAccessException("Tên người dùng hoặc mật khẩu không chính xác");
                }

                var responseModel = await GenerateTokensAsync(user);

                return new Response<LoginResponse>(200, "Success", responseModel);
            }
            catch (UnauthorizedAccessException ex)
            {
                return new Response<LoginResponse>(401, ex.Message, null);
            }
            catch (Exception ex)
            {
                return new Response<LoginResponse>(500, $"Internal server error: {ex.Message}", null);
            }
        }

        public async Task<Response<LoginResponse>> RefreshTokensAsync(string refreshToken)
        {
            try
            {
                var existingRefreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken);

                if (existingRefreshToken == null || !IsRefreshTokenValid(existingRefreshToken))
                {
                    throw new UnauthorizedAccessException("Refresh token không hợp lệ hoặc đã hết hạn.");
                }

                var user = await _userRepository.GetByIdAsync(existingRefreshToken.UserId);

                if (user == null)
                {
                    throw new UnauthorizedAccessException("Người dùng không tồn tại.");
                }

                var responseModel = await GenerateTokensAsync(user);

                // Cập nhật refresh token mới vào cơ sở dữ liệu
                existingRefreshToken.RefeshToken = responseModel.RefreshToken;
                existingRefreshToken.ExpirationTime = DateTime.UtcNow.AddMinutes(JwtTokenService.AccessTokenExpirationMinutes);
                await _refreshTokenRepository.UpdateRefreshTokenAsync(existingRefreshToken);

                return new Response<LoginResponse>(200, "Success", responseModel);
            }
            catch (UnauthorizedAccessException ex)
            {
                return new Response<LoginResponse>(401, ex.Message, null);
            }
            catch (Exception ex)
            {
                return new Response<LoginResponse>(500, $"Internal server error: {ex.Message}", null);
            }
        }

        private async Task<LoginResponse> GenerateTokensAsync(User user)
        {
            var claims = new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role)
    });

            var (accessToken, refreshToken) = _jwtTokenService.GenerateTokens(claims);

            // Lưu refresh token vào cơ sở dữ liệu
            var refreshTokenDTO = new RefeshTokenDTO
            {
                RefeshToken = refreshToken,
                UserId = user.Id,
                ExpirationTime = DateTime.UtcNow.AddMinutes(JwtTokenService.RefreshTokenExpirationDays)
            };

            var refreshTokenEntity = _mapper.Map<RefreshToken>(refreshTokenDTO);
            await _refreshTokenRepository.AddRefreshTokenAsync(refreshTokenEntity);

            var responseModel = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = user.Id,
                Role = user.Role
            };

            return responseModel;
        }

        private bool IsRefreshTokenValid(RefreshToken refreshToken)
        {
            return refreshToken != null && refreshToken.ExpirationTime > DateTime.UtcNow;
        }

        public async Task<Response<string>> LogoutAsync(string accessToken)
        {
            try
            {
                // kiểm tra token
                var claimsPrincipal = _jwtTokenService.GetPrincipalFromToken(accessToken);
                if (claimsPrincipal == null)
                {
                    return new Response<string>(401, "Invalid access token", null);
                }
                // lấy id người dùng từ token
                var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return new Response<string>(401, "Invalid access token", null);
                }
                var accessTokenExpiry = _jwtTokenService.GetExpiryFromToken(accessToken);
                // Tìm refresh token trong cơ sở dữ liệu
                var refreshTokenEntity = await _refreshTokenRepository.GetRefreshTokenByUserIdAsync(userId);
                if (refreshTokenEntity == null)
                {
                    return new Response<string>(401, "Invalid refresh token", null);
                }
                // Thêm access token và refresh token vào danh sách đen
                await _blacklistTokenService.AddToBlacklistAsync(accessToken, accessTokenExpiry - DateTime.UtcNow);
                await _blacklistTokenService.AddToBlacklistAsync(refreshTokenEntity.RefeshToken, refreshTokenEntity.ExpirationTime - DateTime.UtcNow);
                // Xóa refresh token khỏi cơ sở dữ liệu
                await _refreshTokenRepository.DeleteRefreshTokenByUserIdAsync(userId);

                return new Response<string>(200, "Successfully logged out", null);
            }
            catch (Exception ex)
            {
                return new Response<string>(500, $"Internal server error: {ex.Message}", null);
            }
        }
    }
}

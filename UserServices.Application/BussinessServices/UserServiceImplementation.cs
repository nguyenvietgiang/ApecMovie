using ApecCoreIdentity;
using AutoMapper;
using System.Security.Claims;
using UserServices.Application.ModelsDTO;
using UserServices.Domain.Models;
using ApecMovieCore.CoreHasing;
using UserServices.Domain.Interfaces;

namespace UserServices.Application.BussinessServices
{
    public class UserServiceImplementation : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly JwtTokenService _jwtTokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public UserServiceImplementation(IUserRepository userRepository, IMapper mapper, IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _jwtTokenService = new JwtTokenService();
            _refreshTokenRepository = refreshTokenRepository;
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

            var user = _mapper.Map<User>(userDTO);
            user.Id = id;
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            await _userRepository.DeleteAsync(user);
        }


        public async Task<LoginResponse> LoginAsync(LoginRequest loginModel)
        {
            var user = await _userRepository.GetByNameAsync(loginModel.Name);

            if (user == null || user.Password != Encrypt.HashingCore(loginModel.Password))
            {
                throw new UnauthorizedAccessException("Tên người dùng hoặc mật khẩu không chính xác");
            }

            var claims = new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
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

        public async Task<LoginResponse> RefreshTokensAsync(string refreshToken)
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

            var claims = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        });

            var (accessToken, newRefreshToken) = _jwtTokenService.GenerateTokens(claims);

            // Cập nhật refresh token mới vào cơ sở dữ liệu
            existingRefreshToken.RefeshToken = newRefreshToken;
            existingRefreshToken.ExpirationTime = DateTime.UtcNow.AddMinutes(JwtTokenService.AccessTokenExpirationMinutes);
            await _refreshTokenRepository.UpdateRefreshTokenAsync(existingRefreshToken);

            var responseModel = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                UserId = user.Id,
                Role = user.Role
            };

            return responseModel;
        }

        private bool IsRefreshTokenValid(RefreshToken refreshToken)
        {
            return refreshToken != null && refreshToken.ExpirationTime > DateTime.UtcNow;
        }
    }
}

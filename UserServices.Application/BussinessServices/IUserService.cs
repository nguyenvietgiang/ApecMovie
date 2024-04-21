using ApecMovieCore.BaseResponse;
using UserServices.Application.ModelsDTO;

namespace UserServices.Application.BussinessServices
{
    public interface IUserService
    {
        Task<UserDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<UserDTO>> GetAllAsync();
        Task<UserDTO> AddAsync(UserDTO userDTO);
        Task UpdateAsync(Guid id, UserDTO userDTO);
        Task DeleteAsync(Guid id);
        Task<Response<LoginResponse>> LoginAsync(LoginRequest loginRequest);
        Task<Response<LoginResponse>> RefreshTokensAsync(string refreshToken);
    }

}

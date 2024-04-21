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
        Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
        Task<LoginResponse> RefreshTokensAsync(string refreshToken);
    }

}

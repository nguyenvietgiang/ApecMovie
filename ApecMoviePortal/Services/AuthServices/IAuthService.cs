using ApecMoviePortal.Models;

namespace ApecMoviePortal.Services.AuthServices
{
    public interface IAuthService
    {
        Task<bool> RegisterUserAsync(RegisterUserDto registerUserDto);

        Task<ApiResponse<LoginResponse>> LoginAsync(LoginDto loginDto);
    }
}

using ApecMoviePortal.Models;

namespace ApecMoviePortal.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public AuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["BackendApiUrl"];
        }

        public async Task<bool> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/register", registerUserDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginDto loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/login", loginDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
        }
    }
}

using ApecMoviePortal.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

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

        public async Task<UserInfoDto> GetUserInfoAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/profile");
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse<UserInfoDto>>(responseContent);
            return result.Data;
        }

    }
}

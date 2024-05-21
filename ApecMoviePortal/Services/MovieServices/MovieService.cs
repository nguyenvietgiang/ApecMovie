using ApecMoviePortal.Models;
using Newtonsoft.Json;

namespace ApecMoviePortal.Services.MovieServices
{
    public class MovieService : IMovieService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public MovieService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiUrl = configuration["BackendApiUrl"];
        }

        public async Task<Movie> GetMovieByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/movies/{id}"); ;
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse<Movie>>(responseContent);
            return result.Data;
        }

        public async Task<PaginatedResponse<Movie>> GetMoviesAsync()
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/movies");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<PaginatedResponse<Movie>>>(content);

            return apiResponse.Data;
        }


    }
}

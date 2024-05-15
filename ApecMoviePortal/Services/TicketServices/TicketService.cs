using ApecMoviePortal.Models;
using Newtonsoft.Json;
using System.Text;

namespace ApecMoviePortal.Services.TicketServices
{
    public class TicketService : ITicketService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public TicketService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiUrl = configuration["BackendApiUrl"];
        }

        public async Task<bool> ConfirmTicketAsync(Guid ticketId, string token)
        {
            var confirmationDto = new TicketConfirmationDTO
            {
                TicketId = ticketId,
                Token = token
            };

            var content = new StringContent(JsonConvert.SerializeObject(confirmationDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_apiUrl}/confirm", content);

            return response.IsSuccessStatusCode;
        }
    }

}

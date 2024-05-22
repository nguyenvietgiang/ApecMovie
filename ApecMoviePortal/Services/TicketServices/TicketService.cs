﻿using ApecMoviePortal.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
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

        public async Task<List<Ticket>> GetUnpaidTicketsAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{_apiUrl}/unpaid-ticket");
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(jsonString); // In chuỗi JSON ra console để kiểm tra

            return JsonConvert.DeserializeObject<List<Ticket>>(jsonString);
        }

        public async Task<bool> MarkTicketAsPaidAsync(Guid ticketId)
        {
            var url = $"{_apiUrl}/markAsPaid/{ticketId}";

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent("", Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode;
        }

    }

}

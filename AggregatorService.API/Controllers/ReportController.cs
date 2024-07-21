using AggregatorService.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AggregatorService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ReportController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("users-watched-movie/{movieId}")]
        public async Task<IActionResult> GetUsersWatchedMovie(Guid movieId)
        {
            var ticketServiceClient = _httpClientFactory.CreateClient("TicketService");
            var userServiceClient = _httpClientFactory.CreateClient("UserService");

            // Step 1: Get tickets for the movie
            var ticketResponse = await ticketServiceClient.GetAsync($"/api/Ticket/movie-ticket/{movieId}");
            if (!ticketResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)ticketResponse.StatusCode, "Failed to fetch tickets.");
            }
            var tickets = await ticketResponse.Content.ReadAsAsync<List<Ticket>>();

            // Step 2: Extract userIds from tickets
            var userIds = tickets.Select(t => t.UserID).Where(id => id.HasValue).Distinct().ToList();

            // Step 3: Get user details for these userIds
            var users = new List<User>();
            foreach (var userId in userIds)
            {
                if (userId.HasValue)
                {
                    var userResponse = await userServiceClient.GetAsync($"/v1/api/User/{userId.Value}");
                    if (userResponse.IsSuccessStatusCode)
                    {
                        var user = await userResponse.Content.ReadAsAsync<User>();
                        users.Add(user);
                    }
                }
            }

            return Ok(users);
        }
    }
}

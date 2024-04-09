using Microsoft.AspNetCore.Http;


namespace MovieServices.Application.ModelsDTO
{
    public class MovieDTO
    {
        public string? Title { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public string? Director { get; set; }
    }
}

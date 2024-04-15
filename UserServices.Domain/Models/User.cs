using ApecMovieCore;

namespace UserServices.Domain.Models
{
    public class User : BaseModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string Role { get; set; }
    }
}

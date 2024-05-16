using System.ComponentModel.DataAnnotations;

namespace ApecMoviePortal.Models
{ 
    public class LoginDto
    {
        [Required(ErrorMessage = "Tên không được để trống")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(5, ErrorMessage = "Password must be at least 5 characters long")]
        public string Password { get; set; }
    }

}

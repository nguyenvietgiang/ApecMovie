using System.ComponentModel.DataAnnotations;

namespace ApecMoviePortal.Models
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "Tên không được để trống")]
        [MinLength(5, ErrorMessage = "Tên phải có tối thiểu 5 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email sai định dạng")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(5, ErrorMessage = "Mật khẩu phải có tối thiểu 5 ký tự")]
        public string Password { get; set; }
    }
}

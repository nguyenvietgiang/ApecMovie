using FluentValidation;
using UserServices.Application.ModelsDTO;

namespace UserServices.Application.Validator
{
    public class UserDTOValidator : AbstractValidator<UserDTO>
    {
        public UserDTOValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Tên không được trống");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email không được trống").EmailAddress().WithMessage("Email không hợp lệ");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Mật khẩu không được trống").MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự");
        }
    }
}

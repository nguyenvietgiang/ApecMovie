using FluentValidation;
using MovieServices.Application.ModelsDTO;


namespace MovieServices.Application.Validator
{
    public class MovieDTOValidator : AbstractValidator<MovieDTO>
    {
        public MovieDTOValidator()
        {
            RuleFor(m => m.Title)
                .NotEmpty().WithMessage("Tiêu đề không được trống.")
                .MinimumLength(6).WithMessage("Tiêu đề phải tối thiểu 6 ký tự.");

            RuleFor(m => m.Director)
                .NotEmpty().WithMessage("Đạo diễn không được trống.")
                .MinimumLength(6).WithMessage("Tên đạo diễn phải đủ 6 ký tự.");
        }
    }

}

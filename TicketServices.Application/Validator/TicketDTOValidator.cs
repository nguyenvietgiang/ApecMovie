using FluentValidation;
using TicketServices.Application.ModelsDTO;

namespace TicketServices.Application.Validator
{
    public class TicketDTOValidator : AbstractValidator<TicketDTO>
    {
        public TicketDTOValidator()
        {
            RuleFor(ticket => ticket.MovieID).NotEmpty().WithMessage("Id của phim không được để trống");
            RuleFor(ticket => ticket.SeatNumber).GreaterThan(0).WithMessage("Ghế không khả dụng");
            RuleFor(ticket => ticket.ShowTime).NotEmpty().WithMessage("Thời gian không phù hợp");
        }
    }

}

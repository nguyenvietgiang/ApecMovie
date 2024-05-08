using FluentValidation;
using TicketServices.Application.ModelsDTO;

namespace TicketServices.Application.Validator
{
    public class TicketDTOValidator : AbstractValidator<TicketDTO>
    {
        public TicketDTOValidator()
        {
            RuleFor(ticket => ticket.MovieID).NotEmpty().WithMessage("MovieID is required");
            RuleFor(ticket => ticket.SeatNumber).GreaterThan(0).WithMessage("SeatNumber must be greater than 0");
            RuleFor(ticket => ticket.ShowTime).NotEmpty().WithMessage("ShowTime is required");
            RuleFor(ticket => ticket.UserID).NotEmpty().WithMessage("UserID is required");
        }
    }

}

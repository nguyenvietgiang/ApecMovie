using ApecMoviePortal.Models;

namespace ApecMoviePortal.Services.TicketServices
{
    public interface ITicketService
    {
        Task<bool> ConfirmTicketAsync(Guid ticketId, string token);
        Task<List<Ticket>> GetUnpaidTicketsAsync(string token);
        Task<bool> MarkTicketAsPaidAsync(Guid ticketId);
    }
}

using TicketServices.Domain.Models;

namespace TicketServices.Domain.Interface
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetAllTicketsAsync();
        Task<Ticket> GetTicketByIdAsync(Guid id);
        Task AddTicketAsync(Ticket ticket);
        Task UpdateTicketAsync(Ticket ticket);
        Task DeleteTicketAsync(Guid id);
        Task<bool> IsTicketExistsAsync(Guid movieId, int seatNumber, DateTime showTime);
        Task<Ticket?> GetAvailableTicketAsync(Guid movieId, int seatNumber, DateTime showTime);
        Task<Ticket?> GetPendingTicketAsync(Guid movieId, int seatNumber, DateTime showTime);
        Task<IEnumerable<Ticket>> GetUnpaidTicketsByUserAsync(Guid userId);
        Task<IEnumerable<Ticket>> GetTicketbyMovie(Guid movieId);
        Task<bool> UpdatePaymentStatusAsync(Guid ticketId, PaymentStatus newStatus);
    }
}

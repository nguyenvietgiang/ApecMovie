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
    }
}

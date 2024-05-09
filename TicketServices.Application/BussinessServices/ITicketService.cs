using TicketServices.Application.ModelsDTO;
using TicketServices.Domain.Models;

namespace TicketServices.Application.BussinessServices
{
    public interface ITicketService
    {
        Task<IEnumerable<Ticket>> GetAllTicketsAsync();
        Task<Ticket> GetTicketByIdAsync(Guid id);
        Task<Guid> AddTicketAsync(TicketDTO ticketDTO, Guid UserID);
        Task UpdateTicketAsync(Guid id, TicketDTO ticketDTO);
        Task DeleteTicketAsync(Guid id);
        Task<bool> ConfirmTicketAsync(Guid ticketId, string token);
    }
}

using TicketServices.Application.ModelsDTO;
using TicketServices.Domain.Models;

namespace TicketServices.Application.BussinessServices
{
    public interface ITicketService
    {
        Task<IEnumerable<Ticket>> GetAllTicketsAsync();
        Task<Ticket> GetTicketByIdAsync(Guid id);
        Task<Ticket> AddTicketAsync(TicketDTO ticketDTO, Guid UserID);
        Task UpdateTicketAsync(Guid id, TicketDTO ticketDTO);
        Task DeleteTicketAsync(Guid id);
        Task<bool> ConfirmTicketAsync(Guid ticketId, string token, Guid Userid);

        Task<IEnumerable<Ticket>> GetUnpaidTicketsByUserAsync(Guid userId);
        Task<bool> MarkTicketAsPaidAsync(Guid ticketId);

    }
}

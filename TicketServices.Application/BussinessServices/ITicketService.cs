using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketServices.Application.ModelsDTO;

namespace TicketServices.Application.BussinessServices
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDTO>> GetAllTicketsAsync();
        Task<TicketDTO> GetTicketByIdAsync(Guid id);
        Task<Guid> AddTicketAsync(TicketDTO ticketDTO);
        Task UpdateTicketAsync(Guid id, TicketDTO ticketDTO);
        Task DeleteTicketAsync(Guid id);
    }
}

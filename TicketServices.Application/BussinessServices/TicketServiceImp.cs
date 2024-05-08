using AutoMapper;
using ApecMovieCore.Common;
using TicketServices.Application.ModelsDTO;
using TicketServices.Domain.Interface;
using TicketServices.Domain.Models;

namespace TicketServices.Application.BussinessServices
{
    public class TicketServiceImp : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IMapper _mapper; 

        public TicketServiceImp(ITicketRepository ticketRepository, IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TicketDTO>> GetAllTicketsAsync()
        {
            var tickets = await _ticketRepository.GetAllTicketsAsync();
            return _mapper.Map<IEnumerable<TicketDTO>>(tickets);
        }

        public async Task<TicketDTO> GetTicketByIdAsync(Guid id)
        {
            var ticket = await _ticketRepository.GetTicketByIdAsync(id);
            return _mapper.Map<TicketDTO>(ticket);
        }

        public async Task<Guid> AddTicketAsync(TicketDTO ticketDTO)
        {
            var ticket = _mapper.Map<Ticket>(ticketDTO);
            ticket.Id = Guid.NewGuid();
            ticket.Status = false;
            ticket.PaymentStatus = PaymentStatus.Unpaid;
            ticket.Token = Generator.GenerateSixDigitRandomNumber().ToString(); 
            await _ticketRepository.AddTicketAsync(ticket);
            return ticket.Id;
        }

        public async Task UpdateTicketAsync(Guid id, TicketDTO ticketDTO)
        {
            var existingTicket = await _ticketRepository.GetTicketByIdAsync(id);
            if (existingTicket == null)
            {
                throw new ArgumentException("Ticket not found");
            }

            var updatedTicket = _mapper.Map(ticketDTO, existingTicket);
            await _ticketRepository.UpdateTicketAsync(updatedTicket);
        }

        public async Task DeleteTicketAsync(Guid id)
        {
            await _ticketRepository.DeleteTicketAsync(id);
        }
    }
}

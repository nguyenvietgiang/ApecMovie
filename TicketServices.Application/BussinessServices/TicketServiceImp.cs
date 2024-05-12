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

        public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
        {
            var tickets = await _ticketRepository.GetAllTicketsAsync();
            return tickets;
        }

        public async Task<Ticket> GetTicketByIdAsync(Guid id)
        {
            var ticket = await _ticketRepository.GetTicketByIdAsync(id);
            return ticket;
        }

        public async Task<Ticket> AddTicketAsync(TicketDTO ticketDTO, Guid userID)
        {
            if (await _ticketRepository.IsTicketExistsAsync(ticketDTO.MovieID, ticketDTO.SeatNumber, ticketDTO.ShowTime))
            {
                throw new InvalidOperationException("Vé này đã được đặt rồi.");
            }

            var ticket = _mapper.Map<Ticket>(ticketDTO);
            ticket.Id = Guid.NewGuid();
            ticket.Status = false;
            ticket.UserID = userID;
            ticket.PaymentStatus = PaymentStatus.Unpaid;
            ticket.Token = Generator.GenerateSixDigitRandomNumber().ToString();
            await _ticketRepository.AddTicketAsync(ticket);
            return ticket;
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

        public async Task<bool> ConfirmTicketAsync(Guid ticketId, string token)
        {
            var ticket = await _ticketRepository.GetTicketByIdAsync(ticketId);
            // Kiểm tra xem vé có tồn tại không
            if (ticket == null)
            {
                throw new ArgumentException("Ticket not found");
            }
            if (ticket.Token != token)
            {
                throw new ArgumentException("Invalid token");
            }
            ticket.Status = true;
            await _ticketRepository.UpdateTicketAsync(ticket);
            return true;
        }


        public async Task DeleteTicketAsync(Guid id)
        {
            await _ticketRepository.DeleteTicketAsync(id);
        }
    }
}

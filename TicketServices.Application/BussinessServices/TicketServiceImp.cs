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
            try
            {
                // Kiểm tra xem vé đã tồn tại và đã được xác nhận hay chưa
                if (await _ticketRepository.IsTicketExistsAsync(ticketDTO.MovieID, ticketDTO.SeatNumber, ticketDTO.ShowTime))
                {
                    throw new InvalidOperationException("Vé này đã được đặt rồi.");
                }
                // Kiểm tra xem có vé nào phù hợp nhưng chưa được xác nhận hay không
                var existingTicket = await _ticketRepository.GetAvailableTicketAsync(ticketDTO.MovieID, ticketDTO.SeatNumber, ticketDTO.ShowTime);
                if (existingTicket != null)
                {
                    // Nếu có, cập nhật thông tin người dùng và token mới
                    existingTicket.UserID = userID;
                    existingTicket.Token = Generator.GenerateSixDigitRandomNumber().ToString();
                    existingTicket.PaymentStatus = PaymentStatus.Unpaid;
                    await _ticketRepository.UpdateTicketAsync(existingTicket);
                    return existingTicket;
                }
                else
                {
                    // Kiểm tra xem có vé đang đợi xác nhận hay không
                    var pendingTicket = await _ticketRepository.GetPendingTicketAsync(ticketDTO.MovieID, ticketDTO.SeatNumber, ticketDTO.ShowTime);

                    if (pendingTicket != null)
                    {
                        throw new InvalidOperationException("Vé này đang chờ xác nhận.");
                    }
                    else
                    {
                        // Nếu không, tạo vé mới
                        var ticket = _mapper.Map<Ticket>(ticketDTO);
                        ticket.Id = Guid.NewGuid();
                        ticket.Status = false;
                        ticket.UserID = userID;
                        ticket.PaymentStatus = PaymentStatus.Unpaid;
                        ticket.Token = Generator.GenerateSixDigitRandomNumber().ToString();
                        await _ticketRepository.AddTicketAsync(ticket);
                        return ticket;
                    }
                }
            }
            catch (InvalidOperationException)
            {
                throw; 
            }
            catch (Exception)
            {
                throw new Exception("Có lỗi trong quá trình đặt vé, hãy thử lại.");
            }
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

        public async Task<bool> ConfirmTicketAsync(Guid ticketId, string token, Guid Userid)
        {
            var ticket = await _ticketRepository.GetTicketByIdAsync(ticketId);
            // Kiểm tra xem vé có tồn tại không
            if (ticket == null)
            {
                throw new ArgumentException("Vé không khả dụng");
            }
            if (ticket.UserID != Userid)
            {
                throw new ArgumentException("Vé đã được đặt bởi một tài khoản khác");
            }
            if (ticket.Token != token)
            {
                throw new ArgumentException("Đã hết thời hạn đặt vé, hãy đặt lại");
            }
            ticket.Status = true;
            await _ticketRepository.UpdateTicketAsync(ticket);
            return true;
        }

        public async Task<IEnumerable<Ticket>> GetUnpaidTicketsByUserAsync(Guid userId)
        {
            return await _ticketRepository.GetUnpaidTicketsByUserAsync(userId);
        }

        public async Task<bool> MarkTicketAsPaidAsync(Guid ticketId)
        {
            return await _ticketRepository.UpdatePaymentStatusAsync(ticketId, PaymentStatus.Paid);
        }

        public async Task DeleteTicketAsync(Guid id)
        {
            await _ticketRepository.DeleteTicketAsync(id);
        }
    }
}

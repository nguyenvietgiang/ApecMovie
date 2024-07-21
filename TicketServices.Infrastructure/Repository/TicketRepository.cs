using Microsoft.EntityFrameworkCore;
using TicketServices.Domain.Interface;
using TicketServices.Domain.Models;
using TicketServices.Infrastructure.Context;

namespace TicketServices.Infrastructure.Repository
{
    public class TicketRepository : ITicketRepository
    {
        private readonly TicketDbContext _context;

        public TicketRepository(TicketDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
        {
            return await _context.Tickets.ToListAsync();
        }

        public async Task<Ticket> GetTicketByIdAsync(Guid id)
        {
            return await _context.Tickets.FindAsync(id);
        }

        public async Task AddTicketAsync(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            _context.Entry(ticket).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTicketAsync(Guid id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsTicketExistsAsync(Guid movieId, int seatNumber, DateTime showTime)
        {
            return await _context.Tickets.AnyAsync(t =>
                t.MovieID == movieId && t.SeatNumber == seatNumber && t.ShowTime == showTime && t.Status == true);
        }

        public async Task<Ticket?> GetAvailableTicketAsync(Guid movieId, int seatNumber, DateTime showTime)
        {
            return await _context.Tickets.FirstOrDefaultAsync(t => t.MovieID == movieId && t.SeatNumber == seatNumber && t.ShowTime == showTime && t.UserID == null && t.Token == null && t.Status == false);
        }

        public async Task<Ticket?> GetPendingTicketAsync(Guid movieId, int seatNumber, DateTime showTime)
        {
            return await _context.Tickets.FirstOrDefaultAsync(t => t.MovieID == movieId && t.SeatNumber == seatNumber && t.ShowTime == showTime && t.UserID != null && t.Token != null && t.Status == false);
        }

        public async Task<IEnumerable<Ticket>> GetUnpaidTicketsByUserAsync(Guid userId)
        {
            return await _context.Set<Ticket>()
                .Where(t => t.UserID == userId && t.Status==true && t.PaymentStatus == PaymentStatus.Unpaid)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetTicketbyMovie(Guid movieId)
        {
            return await _context.Set<Ticket>()
                .Where(t => t.MovieID == movieId)
                .ToListAsync();
        }

        public async Task<bool> UpdatePaymentStatusAsync(Guid ticketId, PaymentStatus newStatus)
        {
            var ticket = await _context.Set<Ticket>().FindAsync(ticketId);
            if (ticket == null || ticket.PaymentStatus != PaymentStatus.Unpaid || ticket.Status != true)
            {
                return false;
            }

            ticket.PaymentStatus = newStatus;
            _context.Entry(ticket).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }

}

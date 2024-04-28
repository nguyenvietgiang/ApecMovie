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
    }

}

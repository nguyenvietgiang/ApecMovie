using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketServices.Domain.Models;

namespace TicketServices.Infrastructure.Context
{
    public class TicketDbContext : DbContext
    {
        public TicketDbContext()
        {
        }

        public TicketDbContext(DbContextOptions<TicketDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=ApecTicketData;User Id=postgres;Password=vip1111;");
            }
        }

        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}

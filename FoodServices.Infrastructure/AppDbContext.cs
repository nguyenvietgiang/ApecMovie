using FoodServices.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodServices.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<Food> Foods { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Food>()
                .HasKey(f => f.Id);

            modelBuilder.Entity<Food>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();
        }
    }

}

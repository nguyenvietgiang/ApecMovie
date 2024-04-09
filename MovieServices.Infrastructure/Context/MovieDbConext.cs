using Microsoft.EntityFrameworkCore;
using MovieServices.Domain.Models;


namespace MovieServices.Infrastructure.Context
{
    public class MovieDbConext : DbContext
    {
        public MovieDbConext()
        {
        }

        public MovieDbConext(DbContextOptions<MovieDbConext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=ApecMovieData;User Id=postgres;Password=vip1111;");
            }
        }

        public DbSet<Movie> Movies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}

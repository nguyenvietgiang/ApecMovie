using BlogServices.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogServices.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<BlogPost> BlogPosts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogPost>()
                .Property(b => b.ViewCount)
                .HasDefaultValue(0); // Đặt giá trị mặc định cho ViewCount là 0

            modelBuilder.Entity<BlogPost>()
                .Property(b => b.IsActive)
                .HasDefaultValue(true); // Đặt giá trị mặc định cho IsActive là true
        }
    }
}

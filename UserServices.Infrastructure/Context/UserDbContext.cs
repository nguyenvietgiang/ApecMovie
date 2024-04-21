using Microsoft.EntityFrameworkCore;
using UserServices.Domain.Models;

namespace UserServices.Infrastructure.Context
{
    public class UserDbContext : DbContext
    {
        public UserDbContext()
        {
        }

        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=ApecUserData;User Id=postgres;Password=vip1111;");
            }
        }

        public DbSet<User> Users { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // chuyển đổi sang kiểu string khi lưu vào cơ sở dữ liệu và được chuyển đổi lại thành kiểu Guid khi đọc từ cơ sở dữ liệu
            modelBuilder.Entity<RefreshToken>()
                .Property(e => e.UserId)
                .HasConversion(
                    v => v.ToString(),
                    v => Guid.Parse(v))
                .IsRequired();
        }

    }
}

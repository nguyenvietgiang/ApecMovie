using Microsoft.EntityFrameworkCore;
using UserServices.Domain.Interfaces;
using UserServices.Domain.Models;
using UserServices.Infrastructure.Context;

namespace UserServices.Infrastructure.Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly UserDbContext _context;

        public RefreshTokenRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveRefreshTokenAsync(Guid refreshTokenId)
        {
            var refreshToken = await _context.RefreshTokens.FindAsync(refreshTokenId);
            if (refreshToken != null)
            {
                _context.RefreshTokens.Remove(refreshToken);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.RefeshToken == refreshToken);
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken> GetRefreshTokenByUserIdAsync(Guid userId) 
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);
        }

        public async Task DeleteRefreshTokenByUserIdAsync(Guid userId) 
        {
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);
            if (refreshToken != null)
            {
                _context.RefreshTokens.Remove(refreshToken);
                await _context.SaveChangesAsync();
            }
        }
    }
}

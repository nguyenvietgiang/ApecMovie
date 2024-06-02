using UserServices.Domain.Models;

namespace UserServices.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddRefreshTokenAsync(RefreshToken refreshToken);
        Task RemoveRefreshTokenAsync(Guid refreshTokenId);
        Task<RefreshToken> GetRefreshTokenAsync(string refreshToken);
        Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken> GetRefreshTokenByUserIdAsync(Guid userId); // xem refesh token dựa trên id người dùng
        Task DeleteRefreshTokenByUserIdAsync(Guid userId); // khóa khỏi CSDL
    }
}

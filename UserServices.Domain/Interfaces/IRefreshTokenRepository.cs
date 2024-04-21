using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServices.Domain.Models;

namespace UserServices.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddRefreshTokenAsync(RefreshToken refreshToken);
        Task RemoveRefreshTokenAsync(Guid refreshTokenId);
        Task<RefreshToken> GetRefreshTokenAsync(string refreshToken);
        Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
    }
}

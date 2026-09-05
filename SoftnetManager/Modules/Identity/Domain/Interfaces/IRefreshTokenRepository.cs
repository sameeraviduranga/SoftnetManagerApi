using SoftnetManager.Modules.Identity.Domain.Entities;

namespace SoftnetManager.Modules.Identity.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<RefreshToken?> GetStoredRefreshTokenAsync(string hashedToken,string clientId);

        Task RevokedRefreshTokenAsync(RefreshToken refreshToken);
       
    }
}

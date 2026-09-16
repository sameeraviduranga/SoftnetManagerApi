using SoftnetManager.Modules.Identity.Domain.Entities;

namespace SoftnetManager.Modules.Identity.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token,CancellationToken cancellationToken);
        Task<RefreshToken?> GetStoredRefreshTokenAsync(string hashedToken,string clientId, CancellationToken cancellationToken);

        Task RevokedRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
       
    }
}

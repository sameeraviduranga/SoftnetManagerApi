using SoftnetManager.Modules.Identity.Domain.Entities;

namespace SoftnetManager.Modules.Identity.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateJwtTokenAsync(User user,Client client, CancellationToken cancellationToken);
        string GenerateRefreshToken();
        string HashToken(string refreshToken);

    }
}

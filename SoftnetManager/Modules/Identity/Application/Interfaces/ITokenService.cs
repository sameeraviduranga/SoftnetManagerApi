using SoftnetManager.Modules.Identity.Domain.Entities;

namespace SoftnetManager.Modules.Identity.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user,Client client);
        string GenerateRefreshToken();
        string HashToken(string refreshToken);

    }
}

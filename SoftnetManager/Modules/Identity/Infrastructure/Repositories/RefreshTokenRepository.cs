using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SoftnetManager.Modules.Identity.Domain.Entities;
using SoftnetManager.Modules.Identity.Domain.Interfaces;
using SoftnetManager.Modules.Shared.Database;

namespace SoftnetManager.Modules.Identity.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(RefreshToken token)
        {
            await _context.RefreshTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetStoredRefreshTokenAsync(string hashedToken, string clientId)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                    .ThenInclude(u => u.UserProfile)

                .Include(rt => rt.User.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)

                .Include(rt => rt.Client)

                .FirstOrDefaultAsync(rt=>rt.Token == hashedToken && rt.Client.ClientId == clientId);

        }

        public async Task RevokedRefreshTokenAsync(RefreshToken refreshToken)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Id == refreshToken.Id);
            if (token != null)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }

        }
    }
}

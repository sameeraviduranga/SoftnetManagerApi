using Microsoft.EntityFrameworkCore;
using SoftnetManager.Modules.Identity.Domain.Entities;
using SoftnetManager.Modules.Identity.Domain.Interfaces;
using SoftnetManager.Modules.Shared.Database;
using SoftnetManager.Modules.Shared.Repositories;

namespace SoftnetManager.Modules.Identity.Infrastructure.Repositories
{
    public class RoleRepository:GenericRepository<Role>,IRoleRepository
    {
        private readonly AppDbContext context;

        public RoleRepository(AppDbContext context):base(context)
        {
            this.context = context;
        }

        public async Task<bool> AllRoleExistAsync(List<int> roles, CancellationToken cancellationToken)
        {
            if (roles == null || roles.Count == 0)
            {
                return true;
            }
            var count = await context.Roles.CountAsync(r => roles.Contains(r.Id),cancellationToken);
            return count == roles.Count;
        }

        public async Task<Role?> GetRoleByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await context.Roles
                .Include(r=>r.UserRoles)
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == id,cancellationToken);
        }

        public async Task<IEnumerable<RolePermission>> GetRolePermissionAsync(int roleId, CancellationToken cancellationToken)
        {
            return await context.RolePermissions.AsNoTracking()
                .Include(rp => rp.Permission)
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsRegisteredRoleAsync(int roleId, string roleName, CancellationToken cancellationToken)
        {
            
            return await context.Roles.AnyAsync(r => r.Name.ToLower() == roleName.ToLower() && r.Id != roleId,cancellationToken);
        }

        public async Task<bool> IsRoleExistsAsync(string roleName, CancellationToken cancellationToken)
        {
            return await context.Roles.AnyAsync(r => r.Name.ToLower() == roleName.ToLower(),cancellationToken);
        }

    }
}

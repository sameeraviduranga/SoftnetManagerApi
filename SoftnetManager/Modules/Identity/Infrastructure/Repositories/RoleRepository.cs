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

        public async Task<Role?> GetRoleById(int id)
        {
            return await context.Roles
                .Include(r=>r.UserRoles)
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<RolePermission>> GetRolePermissionAsync(int roleId)
        {
            return await context.RolePermission.AsNoTracking()
                .Include(rp => rp.Permission)
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();
        }

        public async Task<bool> IsRegisteredRole(int roleId, string roleName)
        {
            return await context.Roles.AnyAsync(r => r.Name.ToLower() == roleName.ToLower() && r.Id != roleId);
        }

        public async Task<bool> IsRoleExistsAsync(string roleName)
        {
            return await context.Roles.AnyAsync(r => r.Name.ToLower() == roleName.ToLower());
        }

    }
}

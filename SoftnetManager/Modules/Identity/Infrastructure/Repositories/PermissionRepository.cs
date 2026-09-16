using Microsoft.EntityFrameworkCore;
using SoftnetManager.Modules.Identity.Domain.Entities;
using SoftnetManager.Modules.Identity.Domain.Interfaces;
using SoftnetManager.Modules.Shared.Database;
using SoftnetManager.Modules.Shared.Repositories;

namespace SoftnetManager.Modules.Identity.Infrastructure.Repositories
{
    public class PermissionRepository:GenericRepository<Permission>,IPermissionRepository
    {
        private readonly AppDbContext _context;

        public PermissionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> AllExistsAsync(List<int> permissionIds, CancellationToken cancellationToken)
        {
            if (permissionIds == null || permissionIds.Count == 0)
                return true;

            var existingCount = await _context.Permission.CountAsync(p=>permissionIds.Contains(p.Id));
            return existingCount == permissionIds.Count;
        }
    }
}

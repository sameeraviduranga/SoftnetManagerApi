using SoftnetManager.Modules.Identity.Domain.Entities;
using SoftnetManager.Modules.Shared.Interfaces;

namespace SoftnetManager.Modules.Identity.Domain.Interfaces
{
    public interface IRoleRepository:IRepository<Role>
    {
        Task<bool> IsRoleExistsAsync(string roleName,CancellationToken cancellationToken);
        Task<bool> IsRegisteredRoleAsync(int roleId,string roleName, CancellationToken cancellationToken);
        Task<IEnumerable<RolePermission>> GetRolePermissionAsync(int roleId, CancellationToken cancellationToken);
        Task<Role?> GetRoleByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> AllRoleExistAsync(List<int>roles, CancellationToken cancellationToken);
    }
}

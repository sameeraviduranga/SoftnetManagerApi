using SoftnetManager.Modules.Identity.Domain.Entities;
using SoftnetManager.Modules.Shared.Interfaces;

namespace SoftnetManager.Modules.Identity.Domain.Interfaces
{
    public interface IRoleRepository:IRepository<Role>
    {
        Task<bool> IsRoleExistsAsync(string roleName);
        Task<bool> IsRegisteredRole(int roleId,string roleName);
        Task<IEnumerable<RolePermission>> GetRolePermissionAsync(int roleId);
        Task<Role?> GetRoleById(int id);
    }
}

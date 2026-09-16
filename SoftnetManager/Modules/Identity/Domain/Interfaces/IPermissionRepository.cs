using SoftnetManager.Modules.Identity.Domain.Entities;
using SoftnetManager.Modules.Shared.Interfaces;

namespace SoftnetManager.Modules.Identity.Domain.Interfaces
{
    public interface IPermissionRepository:IRepository<Permission>
    {
        Task<bool> AllExistsAsync(List<int>permissionIds,CancellationToken cancellationToken);
    }
}

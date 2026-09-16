using SoftnetManager.Modules.Identity.Application.DTOs.Role;
using SoftnetManager.Modules.Identity.Application.Result;

namespace SoftnetManager.Modules.Identity.Application.Interfaces
{
    public interface IRoleService
    {
        Task<Result<RoleResponseDto>> CreateRoleAsync(CreateRoleDto createRoleDto, CancellationToken cancellationToken);
        Task<Result<RoleResponseDto>> UpdateRoleAsync(UpdateRoleDto updateRoleDto, CancellationToken cancellationToken);
        Task<Result<object>> DeleteRoleAsync(int roleId, CancellationToken cancellationToken);
        Task<Result<IEnumerable<RoleResponseDto>>> GetAllRolesAsync(CancellationToken cancellationToken);
    }
}

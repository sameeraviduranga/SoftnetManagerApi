using SoftnetManager.Modules.Identity.Application.DTOs.Role;
using SoftnetManager.Modules.Identity.Application.Result;

namespace SoftnetManager.Modules.Identity.Application.Interfaces
{
    public interface IRoleService
    {
        Task<Result<RoleResponseDto>> CreateRoleAsync(CreateRoleDto createRoleDto);
        Task<Result<RoleResponseDto>> UpdateRoleAsync(UpdateRoleDto updateRoleDto);
        Task<Result<object>> DeleteRoleAsync(int roleId);
    }
}

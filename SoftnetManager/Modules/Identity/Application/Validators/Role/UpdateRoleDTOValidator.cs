using FluentValidation;
using SoftnetManager.Modules.Identity.Application.DTOs.Role;
using SoftnetManager.Modules.Identity.Domain.Interfaces;

namespace SoftnetManager.Modules.Identity.Application.Validators.Role
{
    public class UpdateRoleDTOValidator:BaseRoleDTOValidator<UpdateRoleDto>
    {
        private readonly IRoleRepository roleRepository;
        private readonly IPermissionRepository permissionRepository;

        public UpdateRoleDTOValidator(IRoleRepository roleRepository,IPermissionRepository permissionRepository):base(permissionRepository)
        {
            this.roleRepository = roleRepository;
            this.permissionRepository = permissionRepository;

            RuleFor(r => r.RoleId)
                .NotEmpty().WithMessage("Role Id required.")
                .GreaterThan(0).WithMessage("Role id is greater than 0")
                .MustAsync(async (roleid, cancellationToken) =>
                {
                    return await roleRepository.ExistsAsync(roleid, cancellationToken);

                }).WithMessage("Role doesn't exist.");

            RuleFor(r => r)
                .MustAsync(async (dto, cancellationToken) =>
                {
                    return !await roleRepository.IsRegisteredRoleAsync(dto.RoleId,dto.Name,cancellationToken);

                }).WithMessage("Role name is already used by another role.");
            
        }
    }
}

using FluentValidation;
using SoftnetManager.Modules.Identity.Application.DTOs.Role;
using SoftnetManager.Modules.Identity.Domain.Entities;
using SoftnetManager.Modules.Identity.Domain.Interfaces;
using SoftnetManager.Modules.Shared.Interfaces;

namespace SoftnetManager.Modules.Identity.Application.Validators.Role
{
    public class CreateRoleDTOValidator:BaseRoleDTOValidator<CreateRoleDto>
    {
        private readonly IRoleRepository roleRepository;
        private readonly IPermissionRepository permissionRepository;

        public CreateRoleDTOValidator(IRoleRepository roleRepository,IPermissionRepository permissionRepository):base(permissionRepository)
        {
            this.roleRepository = roleRepository;
            this.permissionRepository = permissionRepository;

            RuleFor(r=>r.Name)
                .MustAsync(async (rolename, cancellationToken) =>
                {
                    return !await roleRepository.IsRoleExistsAsync(rolename, cancellationToken);

                }).WithMessage("Role name already exist.");

        }


    }
}

using FluentValidation;
using SoftnetManager.Modules.Identity.Application.DTOs.Role;
using SoftnetManager.Modules.Identity.Domain.Interfaces;
using SoftnetManager.Modules.Identity.Infrastructure.Repositories;

namespace SoftnetManager.Modules.Identity.Application.Validators.Role
{
    public class BaseRoleDTOValidator<T>:AbstractValidator<T> where T : CreateRoleDto
    {
        private readonly IPermissionRepository permissionRepository;

        public BaseRoleDTOValidator(IPermissionRepository permissionRepository)
        {
            RuleFor(r => r.Name)
               .NotEmpty().WithMessage("Rolename is required.")
               .MaximumLength(50).WithMessage("Role name can't  exceed 50 charactors");
               

            RuleFor(r => r.Description)
                .MaximumLength(50).WithMessage("Description cannot exceed 50 charactors ")
                .When(r => !string.IsNullOrEmpty(r.Description));

            RuleFor(r => r.Permissions)
                .MustAsync(async (permissions, cancellationToken) =>
                {
                    var distictPermission = permissions.Distinct().ToList();
                    return await permissionRepository.AllExistsAsync(distictPermission, cancellationToken);
                }).WithMessage("One or more permission not exist in database.");
            this.permissionRepository = permissionRepository;
        }
    }
}

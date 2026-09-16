using FluentValidation;
using SoftnetManager.Modules.Identity.Application.DTOs.User;
using SoftnetManager.Modules.Identity.Application.Validators.UserProfile;
using SoftnetManager.Modules.Identity.Domain.Interfaces;

namespace SoftnetManager.Modules.Identity.Application.Validators.user
{
    public class CreateUserDTOValidator:AbstractValidator<CreateUserDTO>
    {
        private readonly IUserRepository userRepository;
        private readonly IRoleRepository roleRepository;
        private readonly ILocationRepository locationRepository;

        public CreateUserDTOValidator(IUserRepository userRepository,IRoleRepository roleRepository,ILocationRepository locationRepository)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
            this.locationRepository = locationRepository;

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Enter valid email")
                .MustAsync(isRegisteredEmail).WithMessage("Email has been already registered.");

            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("password is required")
                .MinimumLength(6).WithMessage("Password should be a minimum of 6 characters in length.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
                .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Password can only contain letters and numbers.");

            //role validation
            RuleFor(u => u.roles)
                .MustAsync(async (roles,cancellationToken) =>
                {
                    return await roleRepository.AllRoleExistAsync(roles.Distinct().ToList(),cancellationToken);
                }).WithMessage("One or More roles doesn't exist ");

            //creatUserprofledto validation
            RuleFor(u => u.UserProfileDto)
                .SetValidator(new CreateUserProfileDTOValidator(userRepository,locationRepository));
        }

        private async Task<bool> isRegisteredEmail(string email, CancellationToken token)
        {
            return !await userRepository.IsEmailExistAsync(email, token);
        }
    }
}

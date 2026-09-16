using FluentValidation;
using SoftnetManager.Modules.Identity.Application.DTOs.LoginAndRegister;
using SoftnetManager.Modules.Identity.Domain.Interfaces;

namespace SoftnetManager.Modules.Identity.Application.Validators
{
    public class RegisterDTOValidator:AbstractValidator<RegisterDTO>
    {
        private readonly IUserRepository userRepository;

        public RegisterDTOValidator(IUserRepository userRepository)
        {

            this.userRepository = userRepository;

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

            RuleFor(u => u.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("Firstname should be maximum 50 charactors long");
            RuleFor(u => u.FirstName)
                .MaximumLength(50).WithMessage("Lastname should be maximum 50 charactors long")
                .When(u => !string.IsNullOrEmpty(u.LastName));


        }

        private async Task<bool> isRegisteredEmail(string email, CancellationToken cancellationToken)
        {
            return !await userRepository.IsEmailExistAsync(email,cancellationToken);
        }
    }
}

using FluentValidation;
using SoftnetManager.Modules.Identity.Application.DTOs.UserProfile;
using SoftnetManager.Modules.Identity.Domain.Interfaces;

namespace SoftnetManager.Modules.Identity.Application.Validators.UserProfile
{
    public class CreateUserProfileDTOValidator:AbstractValidator<CreateUserProfileDTO>
    {

        private readonly string[] _allowedExtentions = { ".jpg", ".jpeg", ".png" };
        private readonly long _maxSizeBytes = 2 * 1024 * 1024;//2MB allowed
        private readonly IUserRepository userRepository;
        private readonly ILocationRepository locationRepository;

        public CreateUserProfileDTOValidator(IUserRepository userRepository,ILocationRepository locationRepository)
        {
            this.userRepository = userRepository;
            this.locationRepository = locationRepository;

            Include(new CreateUpdateUserProfileBaseDTOValidator(userRepository,locationRepository));
            //no extra validation in createuserprofiledto

            //profileimage
            RuleFor(up => up.UserPhoto)
                .Must(file => file?.Length <= _maxSizeBytes).WithMessage("Uploaded image should be less than 2mb")
                .Must(file => file != null && _allowedExtentions.Contains(Path.GetExtension(file.FileName).ToLower()))
                .WithMessage("Only JPG, JPEG, or PNG files are allowed.")
                .Must(file => file?.ContentType != null && file.ContentType.StartsWith("image/")).WithMessage("Please select a valid image file.")
                .When(file => file.UserPhoto != null);

            RuleFor(up=>up.Nic)
                .MustAsync(async (nic, cancellationToken) =>
                {
                    return !await userRepository.IsNicExistAsync(nic, cancellationToken);
                }).WithMessage("Nic is already registered.");


        }
    }
}

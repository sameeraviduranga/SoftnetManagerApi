using FluentValidation;
using SoftnetManager.Modules.Identity.Application.DTOs.UserProfile;
using SoftnetManager.Modules.Identity.Domain.Interfaces;

namespace SoftnetManager.Modules.Identity.Application.Validators.UserProfile
{
    public class UpdateUserProfileDTOValidator:AbstractValidator<UpdateUserProfileDTO>
    {
        private readonly IUserRepository userRepository;
        private readonly ILocationRepository locationRepository;

        public UpdateUserProfileDTOValidator(IUserRepository userRepository,ILocationRepository locationRepository)
        {
            this.userRepository = userRepository;
            this.locationRepository = locationRepository;

            Include(new CreateUpdateUserProfileBaseDTOValidator(userRepository, locationRepository));

            //userid
            RuleFor(up => up.UserID)
                .NotEmpty().WithMessage("User ID is required.")
                .GreaterThan(0).WithMessage("User id greater than 0")
                .MustAsync(async (userid, cancellationToken) =>
                {
                    return await userRepository.ExistsAsync(userid, cancellationToken);
                }).WithMessage("User doen't exist.");

            RuleFor(up=>up.Nic)
                .MustAsync(async (dto,nic,cancellationToken) =>
                {
                    return !await userRepository.CheckRegisteredNicAsync(dto.UserID,nic,cancellationToken);

                }).WithMessage("Nic is already used by another user.");

            //curent profileimage
        }
    }
}

using FluentValidation;
using SoftnetManager.Modules.Identity.Application.DTOs.UserProfile;
using SoftnetManager.Modules.Identity.Domain.Interfaces;

namespace SoftnetManager.Modules.Identity.Application.Validators.UserProfile
{
    public class CreateUpdateUserProfileBaseDTOValidator:AbstractValidator<CreateUpdateUserProfileBaseDTO>
    {
        

       
        private readonly IUserRepository userRepository;
        private readonly ILocationRepository locationRepository;

        public CreateUpdateUserProfileBaseDTOValidator(IUserRepository userRepository,ILocationRepository locationRepository)
        {

            this.userRepository = userRepository;
            this.locationRepository = locationRepository;

            //salutation
            RuleFor(up => up.SalutationID)
                .NotEmpty().WithMessage("Salutation is required.")
                .MustAsync(async (salutationId, cancellationToken) =>
                {
                    return await userRepository.IsSalutationExistAsync(salutationId, cancellationToken);
                })
                .WithMessage("Please Select Salutation");

            //gender
            RuleFor(up => up.GenderID)
                .NotEmpty().WithMessage("Gender is required.")
                .MustAsync(async (genderId, cancellationToken) =>
                {
                    return await userRepository.IsGenderExistAsync(genderId, cancellationToken);
                })
                .WithMessage("Please Select Gender");

            //maritialStatus
            RuleFor(up => up.MaritialStatusID)
                .NotEmpty().WithMessage("MaritialStatus is required.")
                .MustAsync(async (maritialStatusId, cancellationToken) =>
                {
                    return await userRepository.IsMaritialStatusExistAsync(maritialStatusId, cancellationToken);
                })
                .WithMessage("Please Select MaritialStatus");

            //branch
            RuleFor(up => up.BranchID)
                .NotEmpty().WithMessage("Branch is required.")
                .MustAsync(async (branchId, cancellationToken) =>
                {
                    return await userRepository.IsBranchExistAsync(branchId, cancellationToken);
                })
                .WithMessage("Please Select Branch");

            //designation
            RuleFor(up => up.DesignationID)
                .NotEmpty().WithMessage("Designation is required.")
                .MustAsync(async (designationId, cancellationToken) =>
                {
                    return await userRepository.IsDesignationExistAsync(designationId, cancellationToken);
                })
                .WithMessage("Please Select Designation");

            //Firstname
            RuleFor(up => up.FirstName)
                .NotEmpty().WithMessage("FirstName is required.")
                .MaximumLength(50).WithMessage("Firstname should be maximum 50 charactors long");

            //Firstname
            RuleFor(up => up.LastName)
                .NotEmpty().WithMessage("LastName is required.")
                .MaximumLength(50).WithMessage("LastName should be maximum 50 charactors long");

            //phone
            RuleFor(up => up.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^(?:\+94|94|0)?7[0-8][0-9]{7}$").WithMessage("Enter a valid Sri Lankan mobile number.");

            //nic
            RuleFor(up => up.Nic)
                .NotEmpty().WithMessage("NIC is required.");
                

            //dob
            RuleFor(up => up.Dob)
                .Must(dob => dob != default).WithMessage("Date of birth is required.")
                .LessThan(DateOnly.FromDateTime(DateTime.Now)).WithMessage("DOB shouldn't be a future date.");

            
            //child property validation
            RuleFor(up => up.AddressDto)
                .SetValidator(new CreateAddressDTOValidator(locationRepository));
            
        }




    }
}

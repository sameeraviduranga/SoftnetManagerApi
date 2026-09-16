using FluentValidation;
using SoftnetManager.Modules.Identity.Application.DTOs.Address;
using SoftnetManager.Modules.Identity.Domain.Enums;
using SoftnetManager.Modules.Identity.Domain.Interfaces;

namespace SoftnetManager.Modules.Identity.Application.Validators
{
    public class CreateAddressDTOValidator:AbstractValidator<CreateAddressDTO>
    {
        
        public CreateAddressDTOValidator(ILocationRepository locationRepository)
        {

            RuleFor(a => a.ZoneID)
                .GreaterThan(0).WithMessage("Please Select the Zone")
                .MustAsync(async (zoneid, cancellationToken) =>
                {
                    return await locationRepository.IsZoneExistAsync(zoneid, cancellationToken);
                }).WithMessage("Zone doesn't exists.")
                .When(a => a.ZoneID != null);

            RuleFor(a => a.Line1)
                .NotEmpty().WithMessage("Address Line 1 is required.")
                .MaximumLength(20).WithMessage("Maximus length of Address Line 1 should be 20 charactors");

            RuleFor(a => a.Line2)
                .NotEmpty().WithMessage("Address Line 2 is required.")
                .MaximumLength(20).WithMessage("Maximus length of Address Line 2 should be 20 charactors");

            RuleFor(a => a.Line3)
                .MaximumLength(20).WithMessage("Maximus length of Address Line 3 should be 20 charactors")
                .When(a=>!string.IsNullOrEmpty(a.Line3));

            RuleFor(a => a.LocationStatus)
                .NotEmpty().WithMessage("Select the Zone State")
                .IsInEnum().WithMessage("Not valid Zone status");

        }
    }
}

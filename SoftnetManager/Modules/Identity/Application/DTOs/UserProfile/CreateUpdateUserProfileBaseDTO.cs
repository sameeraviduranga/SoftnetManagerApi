using SoftnetManager.Modules.Identity.Application.DTOs.Address;
using System.ComponentModel.DataAnnotations;

namespace SoftnetManager.Modules.Identity.Application.DTOs.UserProfile
{
    public class CreateUpdateUserProfileBaseDTO
    {
        
        public int SalutationID { get; set; }
        public int GenderID { get; set; }
        public int MaritialStatusID { get; set; }
        public int BranchID { get; set; }
        public int DesignationID { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public CreateAddressDTO AddressDto { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Nic { get; set; } = null!;
        public DateOnly Dob { get; set; }
        
    }
}

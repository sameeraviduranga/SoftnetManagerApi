using SoftnetManager.Modules.Identity.Application.DTOs.Address;
using System.ComponentModel.DataAnnotations;
namespace SoftnetManager.Modules.Identity.Application.DTOs.UserProfile
{
    public class UpdateProfileDTO
    {
        [Required(ErrorMessage = "Salutation is required.")]
        public int SalutationID { get; set; }
        [Required(ErrorMessage = "Gender is required.")]
        public int GenderID { get; set; }
        [Required(ErrorMessage = "Marital Status is required.")]
        public int MaritialStatusID { get; set; }
        [Required(ErrorMessage = "Branch is required.")]
        public int BranchID { get; set; }
        [Required(ErrorMessage = "Designation is required.")]
        public int DesignationID { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        public CreateAddressDTO Address { get; set; } = null!;

        [MaxLength(50, ErrorMessage = "First name must be less than or equal to 50 characters.")]
        public string FirstName { get; set; } = string.Empty;
        [MaxLength(50, ErrorMessage = "Last name must be less than or equal to 50 characters.")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage ="invalid phone number")]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required(ErrorMessage = "Nic is required.")]
        [MaxLength(10,ErrorMessage = "Nic must be less than 10 charactors ")]
        public string Nic { get; set; } = string.Empty;
        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime Dob { get; set; }
        public string? ProfileImageUrl { get; set; }

    }
}
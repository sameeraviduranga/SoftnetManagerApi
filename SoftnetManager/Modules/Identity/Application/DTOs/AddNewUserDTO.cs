using SoftnetManager.Modules.Identity.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SoftnetManager.Modules.Identity.Application.DTOs
{
    public class AddNewUserDTO
    {
        public int? SalutationID { get; set; }
        public int? GenderID { get; set; }
        public int? MaritialStatusID { get; set; }

        public int? AddressID { get; set; }

        public int? BranchID { get; set; }

        public int? DesignationID { get; set; }
        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50, ErrorMessage = "First name must be less than or equal to 50 characters.")]
        public string FirstName { get; set; } = string.Empty;
        [MaxLength(50, ErrorMessage = "Last name must be less than or equal to 50 characters.")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [MaxLength(100, ErrorMessage = "Email must be less than or equal to 100 characters.")]
        public string Email { get; set; } = string.Empty;
        [Phone]
        public string? PhoneNumber { get; set; }
        public string? Nic { get; set; }
        public DateTime? Dob { get; set; }
        public string? ProfileImageUrl { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [MaxLength(100, ErrorMessage = "Password must be less than or equal to 100 characters.")]
        public string Password { get; set; } = string.Empty;
       
    }
}

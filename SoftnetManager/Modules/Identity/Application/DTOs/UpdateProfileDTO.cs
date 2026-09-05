using System.ComponentModel.DataAnnotations;
namespace SoftnetManager.Modules.Identity.Application.DTOs
{
    public class UpdateProfileDTO
    {
        public int? SalutationID { get; set; }
        public int? GenderID { get; set; }
        public int? MaritialStatusID { get; set; }
        public int? AddressID { get; set; }
        public int? BranchID { get; set; }
        public int? DesignationID { get; set; }

        [MaxLength(50, ErrorMessage = "First name must be less than or equal to 50 characters.")]
        public string? FirstName { get; set; }
        [MaxLength(50, ErrorMessage = "Last name must be less than or equal to 50 characters.")]
        public string? LastName { get; set; }
        [Phone(ErrorMessage ="invalid phone number")]
        public string? PhoneNumber { get; set; }
        [MaxLength(10,ErrorMessage = "Nic must be less than 10 charactors ")]
        public string? Nic { get; set; }
        public DateTime? Dob { get; set; }    
        
    }
}
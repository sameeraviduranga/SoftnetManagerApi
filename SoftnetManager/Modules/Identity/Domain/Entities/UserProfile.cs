using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SoftnetManager.Modules.Identity.Domain.Entities
{
    [Index(nameof(Nic),Name ="IX_User_Nic", IsUnique = true)]
    public class UserProfile
    {
        [Key]
        public int ID { get; set; }

        public int? SalutationID { get; set; }
        public Salutation? Salutation { get; set; }

        public int? GenderID { get; set; }
        public Gender? Gender { get; set; }

        public int? MaritialStatusID { get; set; }
        public MaritialStatus? MaritialStatus { get; set; }

        public int? AddressID { get; set; }
        public Address? Address { get; set; }

        public int? BranchID { get; set; }
        public Branch? Branch { get; set; }

        public int? DesignationID { get; set; }
        public Designation? Designation { get; set; }

        [Required(ErrorMessage ="First name is required")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        [StringLength(50)]
        public string? LastName { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
        public string? Nic { get; set; }
        public DateTime? Dob { get; set; }
        public string? ProfileImageUrl { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? DeactivatedBy { get; set; }
        public DateTime? DeactivatedAt { get; set; }
       
    }
}

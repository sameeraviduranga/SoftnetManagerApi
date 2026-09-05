namespace SoftnetManager.Modules.Identity.Application.DTOs
{
    public class UserProfileDTO
    {
        public string? Salutation { get; set; }
        public string? Gender { get; set; }
        public string? MaritialStatus { get; set; }
        public AddressDTO? Address { get; set; }
        public string? Branch { get; set; }
        public string? Designation { get; set; }
        public string? FirstName { get; set; }
        public string? Lastname { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Nic { get; set; }
        public string? Dob { get; set; }
        public string? ProfileImageUrl { get; set; }
        public bool IsActive { get; set; }

    }
}

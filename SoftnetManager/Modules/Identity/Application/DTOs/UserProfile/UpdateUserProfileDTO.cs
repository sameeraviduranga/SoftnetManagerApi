namespace SoftnetManager.Modules.Identity.Application.DTOs.UserProfile
{
    public class UpdateUserProfileDTO:CreateUpdateUserProfileBaseDTO
    {
        public int UserID { get; set; }
        public string? CurrentProfileImageName { get; set; }
    }
}

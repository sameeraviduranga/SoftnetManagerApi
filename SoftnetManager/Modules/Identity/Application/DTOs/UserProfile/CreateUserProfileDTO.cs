namespace SoftnetManager.Modules.Identity.Application.DTOs.UserProfile
{
    public class CreateUserProfileDTO:CreateUpdateUserProfileBaseDTO
    {
        public IFormFile? UserPhoto { get; set; }//only for creat not use in update
    }
}

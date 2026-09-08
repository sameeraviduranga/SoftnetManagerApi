using SoftnetManager.Modules.Identity.Application.DTOs.UserProfile;
using SoftnetManager.Modules.Identity.Domain.Entities;

namespace SoftnetManager.Modules.Identity.Application.DTOs.User
{
    public class UserDTO
    {
        public UserProfileDTO UserProfile { get; set; } = null!;
        public string Email { get; set; } = string.Empty;
        public List<string> UserRoles { get; set; } = null!;//NO NEED FOR EXAMPLE
    }
}

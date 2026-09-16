using SoftnetManager.Modules.Identity.Application.DTOs.UserProfile;
using SoftnetManager.Modules.Identity.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SoftnetManager.Modules.Identity.Application.DTOs.User
{
    public class CreateUserDTO
    {
        public CreateUserProfileDTO UserProfileDto { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public List<int> roles { get; set; } = new List<int>();
    }
}

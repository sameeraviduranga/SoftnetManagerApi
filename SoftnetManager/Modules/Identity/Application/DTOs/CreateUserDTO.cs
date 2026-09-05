using SoftnetManager.Modules.Identity.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SoftnetManager.Modules.Identity.Application.DTOs
{
    public class CreateUserDTO
    {
        public CreateUserProfileDTO ProfileDTO { get; set; } = null!;
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [MaxLength(50,ErrorMessage = "Email cannot exceed 50 characters")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; } = null!;
        public List<int> roles { get; set; } = new List<int>();
    }
}

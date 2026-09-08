using System.ComponentModel.DataAnnotations;

namespace SoftnetManager.Modules.Identity.Application.DTOs.UserProfile
{
    public class ToggleActiveStatusDTO
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace SoftnetManager.Modules.Identity.Application.DTOs.Role
{
    public class CreateRoleDto
    {
        [Required]
        [StringLength(50,ErrorMessage ="Role name must be between 2 and 50 characters.")]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<int> Permissions { get; set; } = new List<int>();

    }
}

using System.ComponentModel.DataAnnotations;

namespace SoftnetManager.Modules.Identity.Application.DTOs.Role
{
    public class CreateRoleDto
    {
        
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<int> Permissions { get; set; } = new List<int>();

    }
}

namespace SoftnetManager.Modules.Identity.Application.DTOs.Role
{
    public class RoleResponseDto
    {
        public string RoleName { get; set; } = null!;
        public string? Description { get; set; }
        public List<string> RolePermissions { get; set; } = new List<string>();

    }
}

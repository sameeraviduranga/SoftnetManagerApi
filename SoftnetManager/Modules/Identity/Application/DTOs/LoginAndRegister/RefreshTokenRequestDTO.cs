using System.ComponentModel.DataAnnotations;

namespace SoftnetManager.Modules.Identity.Application.DTOs.LoginAndRegister
{
    public class RefreshTokenRequestDTO
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
        [Required]
        public string ClientId { get; set; } = string.Empty;
    }
}

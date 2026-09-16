namespace SoftnetManager.Modules.Identity.Application.DTOs.LoginAndRegister
{
    public class TokenResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}

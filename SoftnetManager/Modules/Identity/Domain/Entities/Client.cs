using System.ComponentModel.DataAnnotations;

namespace SoftnetManager.Modules.Identity.Domain.Entities
{
    public class Client
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ClientId { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string ClientURL { get; set; } = string.Empty;
        public ICollection<RefreshToken> RefreshTokens = new List<RefreshToken>();
    }
}

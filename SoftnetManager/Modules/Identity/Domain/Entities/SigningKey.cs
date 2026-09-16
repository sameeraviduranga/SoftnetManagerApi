using System.ComponentModel.DataAnnotations;

namespace SoftnetManager.Modules.Identity.Domain.Entities
{
    public class SigningKey
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string KeyId { get; set; } = string.Empty;
        [Required]
        public string PrivateKey { get; set; } = string.Empty;
        [Required]
        public string PublicKey { get; set; } = string.Empty;
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime ExpiresAt { get; set; }
    }
}

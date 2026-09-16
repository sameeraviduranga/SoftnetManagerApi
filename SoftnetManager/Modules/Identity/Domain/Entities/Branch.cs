using SoftnetManager.Modules.Identity.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SoftnetManager.Modules.Identity.Domain.Entities
{
    public class Branch
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public Address Address { get; set; } = null!;//navigation property
        public ICollection<UserProfile> users { get; set; } = new List<UserProfile>();

    }
}

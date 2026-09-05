using SoftnetManager.Modules.Identity.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SoftnetManager.Modules.Identity.Domain.Entities
{
    public class Branch
    {
        [Key]
        public int ID { get; set; }
        public int AddressID { get; set; }
        public Address Address { get; set; } = null!;
        [Required]
        public string Name { get; set; } = string.Empty;

        public ICollection<UserProfile> users { get; set; } = new List<UserProfile>();

    }
}

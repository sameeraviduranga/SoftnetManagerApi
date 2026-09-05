using SoftnetManager.Modules.Identity.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftnetManager.Modules.Identity.Domain.Entities
{
    public class Address
    {
        [Key]
        public int ID { get; set; }
        public int? ZoneID { get; set; }
        public Zone? Zone { get; set; }
        [Required]
        public string Line1 { get; set; } = string.Empty;
        [Required]
        public string Line2 { get; set; } = string.Empty;
        public string? Line3 { get; set; }
        [Required]
        public LocationStatus LocationStatus { get; set; }//home or office use enum
        
    }
}

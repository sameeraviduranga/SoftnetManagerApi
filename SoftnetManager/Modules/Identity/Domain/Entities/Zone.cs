using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftnetManager.Modules.Identity.Domain.Entities
{
    public class Zone
    {
        [Key]
        public int ID { get; set; }
        
        [Required]
        public int CityID { get; set; }
        public City City { get; set; } = null!;
        [Required]
        public string ZoneName { get; set; } = string.Empty;
        
    }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftnetManager.Modules.Identity.Domain.Entities
{
    public class City
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int ProvinceID { get; set; }
        public Province Province { get; set; } = null!;
        [Required]
        public string CityName { get; set; } = string.Empty;
        public ICollection<Zone> Zones { get; set; } = new List<Zone>();
    }
}

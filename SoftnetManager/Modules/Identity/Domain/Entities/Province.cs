using System.ComponentModel.DataAnnotations;

namespace SoftnetManager.Modules.Identity.Domain.Entities
{
    public class Province
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string ProvinceName { get; set; } = string.Empty;
        public ICollection<City> Cities { get; set; } = new List<City>();
    }
}

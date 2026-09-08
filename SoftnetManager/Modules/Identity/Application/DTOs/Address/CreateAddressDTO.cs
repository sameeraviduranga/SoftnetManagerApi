using SoftnetManager.Modules.Identity.Domain.Entities;
using SoftnetManager.Modules.Identity.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SoftnetManager.Modules.Identity.Application.DTOs.Address
{
    public class CreateAddressDTO
    {
        [Required(ErrorMessage ="Line1 is required")]
        public string Line1 { get; set; } = string.Empty;
        [Required(ErrorMessage ="Line2 is required")]
        public string Line2 { get; set; } = string.Empty;
        public string? Line3 { get; set; }
        [Required]
        public LocationStatus LocationStatus { get; set; }
    }
}

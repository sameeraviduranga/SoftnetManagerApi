using SoftnetManager.Modules.Identity.Domain.Entities;
using SoftnetManager.Modules.Identity.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SoftnetManager.Modules.Identity.Application.DTOs.Address
{
    public class CreateAddressDTO
    {
        public int? ZoneID { get; set; }
        public string Line1 { get; set; } = string.Empty;
        public string Line2 { get; set; } = string.Empty;
        public string? Line3 { get; set; }
        public LocationStatus LocationStatus { get; set; }
    }
}

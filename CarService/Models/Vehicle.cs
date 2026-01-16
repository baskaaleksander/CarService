using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarService.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [Required]
        [StringLength(17, MinimumLength = 17)]
        public string VIN { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^[A-Z]{1,3}[0-9A-Z]{4,5}$")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required]
        public string OwnerId { get; set; } = string.Empty;

        public ApplicationUser? Owner { get; set; }

        public ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CarService.Validation;

namespace CarService.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        [StringLength(50, ErrorMessage = "Brand cannot exceed 50 characters")]
        [Display(Name = "Brand")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model is required")]
        [StringLength(50, ErrorMessage = "Model cannot exceed 50 characters")]
        [Display(Name = "Model")]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "VIN is required")]
        [VinValidation]
        [Display(Name = "VIN")]
        public string VIN { get; set; } = string.Empty;

        [Required(ErrorMessage = "Registration number is required")]
        [PolishRegistration]
        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required]
        public string OwnerId { get; set; } = string.Empty;

        public ApplicationUser? Owner { get; set; }

        public ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();
    }
}

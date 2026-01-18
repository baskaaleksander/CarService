using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CarService.Validation;

namespace CarService.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Marka jest wymagana")]
        [StringLength(50, ErrorMessage = "Marka nie może przekraczać 50 znaków")]
        [Display(Name = "Marka")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model jest wymagany")]
        [StringLength(50, ErrorMessage = "Model nie może przekraczać 50 znaków")]
        [Display(Name = "Model")]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "VIN jest wymagany")]
        [VinValidation]
        [Display(Name = "VIN")]
        public string VIN { get; set; } = string.Empty;

        [Required(ErrorMessage = "Numer rejestracyjny jest wymagany")]
        [PolishRegistration]
        [Display(Name = "Numer rejestracyjny")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Właściciel jest wymagany")]
        public string OwnerId { get; set; } = string.Empty;

        public ApplicationUser? Owner { get; set; }

        public ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();
    }
}

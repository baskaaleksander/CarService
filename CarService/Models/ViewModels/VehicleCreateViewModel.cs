using System.ComponentModel.DataAnnotations;

namespace CarService.Models.ViewModels
{
    public class VehicleCreateViewModel
    {
        [Required(ErrorMessage = "Marka jest wymagana")]
        [StringLength(50, ErrorMessage = "Marka nie może przekraczać 50 znaków")]
        [Display(Name = "Marka")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model jest wymagany")]
        [StringLength(50, ErrorMessage = "Model nie może przekraczać 50 znaków")]
        [Display(Name = "Model")]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "VIN jest wymagany")]
        [StringLength(17, MinimumLength = 17, ErrorMessage = "VIN musi mieć dokładnie 17 znaków")]
        [Display(Name = "VIN")]
        public string VIN { get; set; } = string.Empty;

        [Required(ErrorMessage = "Numer rejestracyjny jest wymagany")]
        [RegularExpression("^[A-Z]{1,3}[0-9A-Z]{4,5}$", ErrorMessage = "Nieprawidłowy format numeru rejestracyjnego")]
        [Display(Name = "Numer rejestracyjny")]
        public string RegistrationNumber { get; set; } = string.Empty;
    }
}

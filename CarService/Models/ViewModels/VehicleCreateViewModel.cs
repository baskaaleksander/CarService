using System.ComponentModel.DataAnnotations;

namespace CarService.Models.ViewModels
{
    public class VehicleCreateViewModel
    {
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
    }
}

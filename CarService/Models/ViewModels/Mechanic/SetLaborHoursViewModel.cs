using System.ComponentModel.DataAnnotations;

namespace CarService.Models.ViewModels.Mechanic
{
    public class SetLaborHoursViewModel
    {
        public int OrderId { get; set; }
        public decimal CurrentHours { get; set; }

        [Required]
        [Range(0.25, 1000, ErrorMessage = "Hours must be at least 0.25")]
        public decimal Hours { get; set; }
    }
}

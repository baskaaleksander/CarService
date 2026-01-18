using System.ComponentModel.DataAnnotations;

namespace CarService.Models.ViewModels.Mechanic
{
    public class SetLaborHoursViewModel
    {
        public int OrderId { get; set; }
        public decimal CurrentHours { get; set; }

        [Required(ErrorMessage = "Godziny pracy są wymagane")]
        [Range(0.25, 1000, ErrorMessage = "Godziny pracy muszą być w zakresie od 0,25 do 1000")]
        [Display(Name = "Godziny pracy")]
        public decimal Hours { get; set; }
    }
}

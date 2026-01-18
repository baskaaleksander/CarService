using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarService.Models.ViewModels.Mechanic
{
    public class AddPartItemViewModel
    {
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Część jest wymagana")]
        [Display(Name = "Część")]
        public int PartId { get; set; }

        [Required(ErrorMessage = "Ilość jest wymagana")]
        [Range(1, 100, ErrorMessage = "Ilość musi być w zakresie od 1 do 100")]
        [Display(Name = "Ilość")]
        public int Quantity { get; set; } = 1;

        public IEnumerable<SelectListItem> AvailableParts { get; set; } = new List<SelectListItem>();
    }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarService.Models.ViewModels.Mechanic
{
    public class AddServiceItemViewModel
    {
        public int OrderId { get; set; }

        [Required]
        [Display(Name = "Service")]
        public int ServiceId { get; set; }

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; } = 1;

        public IEnumerable<SelectListItem> AvailableServices { get; set; } = new List<SelectListItem>();
    }
}

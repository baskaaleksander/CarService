using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarService.Models.ViewModels.Mechanic
{
    public class AddPartItemViewModel
    {
        public int OrderId { get; set; }

        [Required]
        [Display(Name = "Part")]
        public int PartId { get; set; }

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; } = 1;

        public IEnumerable<SelectListItem> AvailableParts { get; set; } = new List<SelectListItem>();
    }
}

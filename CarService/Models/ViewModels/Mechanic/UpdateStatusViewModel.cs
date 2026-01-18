using System.ComponentModel.DataAnnotations;
using CarService.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarService.Models.ViewModels.Mechanic
{
    public class UpdateStatusViewModel
    {
        public int OrderId { get; set; }
        public ServiceOrderStatus CurrentStatus { get; set; }

        [Required(ErrorMessage = "Nowy status jest wymagany")]
        [Display(Name = "Nowy status")]
        public ServiceOrderStatus NewStatus { get; set; }

        public IEnumerable<SelectListItem> AvailableStatuses { get; set; } = new List<SelectListItem>();
    }
}

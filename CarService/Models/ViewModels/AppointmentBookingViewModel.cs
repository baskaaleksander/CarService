using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarService.Models.ViewModels
{
    public class AppointmentBookingViewModel
    {
        [Required(ErrorMessage = "Pojazd jest wymagany")]
        [Display(Name = "Pojazd")]
        public int VehicleId { get; set; }

        [StringLength(2000, ErrorMessage = "Opis / uwagi nie mogą przekraczać 2000 znaków")]
        [Display(Name = "Opis / uwagi")]
        public string? Description { get; set; }

        [Display(Name = "Usługi")]
        public List<int>? SelectedServiceIds { get; set; }

        // For populating dropdowns
        public IEnumerable<SelectListItem> AvailableVehicles { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> AvailableServices { get; set; } = new List<SelectListItem>();
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarService.Models.ViewModels
{
    public class AppointmentBookingViewModel
    {
        [Required]
        [Display(Name = "Vehicle")]
        public int VehicleId { get; set; }

        [StringLength(2000)]
        [Display(Name = "Description / Notes")]
        public string? Description { get; set; }

        [Display(Name = "Services")]
        public List<int>? SelectedServiceIds { get; set; }

        // For populating dropdowns
        public IEnumerable<SelectListItem> AvailableVehicles { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> AvailableServices { get; set; } = new List<SelectListItem>();
    }
}

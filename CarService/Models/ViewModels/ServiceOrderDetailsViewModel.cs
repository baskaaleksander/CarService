using System;
using System.Collections.Generic;

namespace CarService.Models.ViewModels
{
    public class ServiceOrderDetailsViewModel
    {
        public int Id { get; set; }
        public string VehicleInfo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public decimal TotalCost { get; set; }
        public string? DiagnosticNotes { get; set; }
        public string? MechanicName { get; set; }
        public List<ServiceOrderItemViewModel> Items { get; set; } = new();
        public bool CanReview { get; set; }
        public ReviewViewModel? Review { get; set; }
    }
}

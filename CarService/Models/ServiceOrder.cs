using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CarService.Models.Enums;

namespace CarService.Models
{
    public class ServiceOrder
    {
        public int Id { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Required]
        public string ClientId { get; set; } = string.Empty;

        public string? MechanicId { get; set; }

        [Required]
        [Display(Name = "Status")]
        public ServiceOrderStatus Status { get; set; } = ServiceOrderStatus.Pending;

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Completed At")]
        public DateTime? CompletedAt { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Total cost cannot be negative")]
        [DataType(DataType.Currency)]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }

        [StringLength(2000, ErrorMessage = "Diagnostic notes cannot exceed 2000 characters")]
        [Display(Name = "Diagnostic Notes")]
        public string? DiagnosticNotes { get; set; }

        [Range(0, 1000, ErrorMessage = "Labor hours must be between 0 and 1000")]
        [Display(Name = "Labor Hours")]
        public decimal LaborHours { get; set; }

        public Vehicle? Vehicle { get; set; }

        public ApplicationUser? Client { get; set; }

        public ApplicationUser? Mechanic { get; set; }

        public ICollection<ServiceOrderItem> Items { get; set; } = new List<ServiceOrderItem>();

        public Review? Review { get; set; }
    }
}

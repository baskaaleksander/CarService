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
        public ServiceOrderStatus Status { get; set; } = ServiceOrderStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        public decimal TotalCost { get; set; }

        [StringLength(2000)]
        public string? DiagnosticNotes { get; set; }

        [Range(0, double.MaxValue)]
        public decimal LaborHours { get; set; }

        public Vehicle? Vehicle { get; set; }

        public ApplicationUser? Client { get; set; }

        public ApplicationUser? Mechanic { get; set; }

        public ICollection<ServiceOrderItem> Items { get; set; } = new List<ServiceOrderItem>();

        public Review? Review { get; set; }
    }
}

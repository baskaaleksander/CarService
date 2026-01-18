using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CarService.Models.Enums;

namespace CarService.Models
{
    public class ServiceOrder
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Pojazd jest wymagany")]
        public int VehicleId { get; set; }

        [Required(ErrorMessage = "Klient jest wymagany")]
        public string ClientId { get; set; } = string.Empty;

        public string? MechanicId { get; set; }

        [Required(ErrorMessage = "Status jest wymagany")]
        [Display(Name = "Status")]
        public ServiceOrderStatus Status { get; set; } = ServiceOrderStatus.Pending;

        [Display(Name = "Data utworzenia")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Data zakończenia")]
        public DateTime? CompletedAt { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Koszt całkowity nie może być ujemny")]
        [DataType(DataType.Currency)]
        [Display(Name = "Koszt całkowity")]
        public decimal TotalCost { get; set; }

        [StringLength(2000, ErrorMessage = "Notatki diagnostyczne nie mogą przekraczać 2000 znaków")]
        [Display(Name = "Notatki diagnostyczne")]
        public string? DiagnosticNotes { get; set; }

        [Range(0, 1000, ErrorMessage = "Godziny pracy muszą być w zakresie od 0 do 1000")]
        [Display(Name = "Godziny pracy")]
        public decimal LaborHours { get; set; }

        public Vehicle? Vehicle { get; set; }

        public ApplicationUser? Client { get; set; }

        public ApplicationUser? Mechanic { get; set; }

        public ICollection<ServiceOrderItem> Items { get; set; } = new List<ServiceOrderItem>();

        public Review? Review { get; set; }
    }
}

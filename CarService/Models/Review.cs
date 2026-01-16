using System;
using System.ComponentModel.DataAnnotations;

namespace CarService.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int ServiceOrderId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ServiceOrder? ServiceOrder { get; set; }
    }
}

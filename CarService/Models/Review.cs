using System;
using System.ComponentModel.DataAnnotations;
using CarService.Validation;

namespace CarService.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int ServiceOrderId { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [RatingRange]
        [Display(Name = "Rating")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        [Display(Name = "Comment")]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ServiceOrder? ServiceOrder { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using CarService.Validation;

namespace CarService.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Zlecenie serwisowe jest wymagane")]
        public int ServiceOrderId { get; set; }

        [Required(ErrorMessage = "Ocena jest wymagana")]
        [RatingRange]
        [Display(Name = "Ocena")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Komentarz nie może przekraczać 1000 znaków")]
        [Display(Name = "Komentarz")]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ServiceOrder? ServiceOrder { get; set; }
    }
}

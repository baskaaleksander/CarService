using System.ComponentModel.DataAnnotations;

namespace CarService.Models.ViewModels
{
    public class ReviewCreateViewModel
    {
        public int ServiceOrderId { get; set; }
        public string VehicleInfo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ocena jest wymagana")]
        [Range(1, 5, ErrorMessage = "Ocena musi być w zakresie od 1 do 5")]
        [Display(Name = "Ocena")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Komentarz nie może przekraczać 1000 znaków")]
        [Display(Name = "Komentarz")]
        public string? Comment { get; set; }
    }
}

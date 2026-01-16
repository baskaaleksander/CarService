using System.ComponentModel.DataAnnotations;

namespace CarService.Models.ViewModels
{
    public class ReviewCreateViewModel
    {
        public int ServiceOrderId { get; set; }
        public string VehicleInfo { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        [Display(Name = "Rating")]
        public int Rating { get; set; }

        [StringLength(1000)]
        [Display(Name = "Comment")]
        public string? Comment { get; set; }
    }
}

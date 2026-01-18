using System.Collections.Generic;

namespace CarService.Models.ViewModels
{
    public class HomeIndexViewModel
    {
        public double AverageRating { get; set; }
        public List<HomeTestimonialViewModel> Testimonials { get; set; } = new();
    }
}

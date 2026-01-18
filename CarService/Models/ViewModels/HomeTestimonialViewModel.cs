using System;

namespace CarService.Models.ViewModels
{
    public class HomeTestimonialViewModel
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string ClientName { get; set; } = "Klient";
        public DateTime CreatedAt { get; set; }
    }
}

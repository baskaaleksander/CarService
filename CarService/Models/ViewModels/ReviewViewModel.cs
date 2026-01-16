using System;

namespace CarService.Models.ViewModels
{
    public class ReviewViewModel
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

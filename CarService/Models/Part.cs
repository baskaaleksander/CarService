using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarService.Models
{
    public class Part
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Part name is required")]
        [StringLength(100, ErrorMessage = "Part name cannot exceed 100 characters")]
        [Display(Name = "Part Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Unit price is required")]
        [Range(0.01, 100000, ErrorMessage = "Unit price must be between 0.01 and 100,000")]
        [DataType(DataType.Currency)]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        public ICollection<ServiceOrderItem> ServiceOrderItems { get; set; } = new List<ServiceOrderItem>();
    }
}

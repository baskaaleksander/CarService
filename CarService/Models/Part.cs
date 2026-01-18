using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarService.Models
{
    public class Part
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa części jest wymagana")]
        [StringLength(100, ErrorMessage = "Nazwa części nie może przekraczać 100 znaków")]
        [Display(Name = "Nazwa części")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ilość na stanie jest wymagana")]
        [Range(0, int.MaxValue, ErrorMessage = "Ilość na stanie nie może być ujemna")]
        [Display(Name = "Ilość na stanie")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Cena jednostkowa jest wymagana")]
        [Range(0.01, 100000, ErrorMessage = "Cena jednostkowa musi być w zakresie od 0,01 do 100 000")]
        [DataType(DataType.Currency)]
        [Display(Name = "Cena jednostkowa")]
        public decimal UnitPrice { get; set; }

        public ICollection<ServiceOrderItem> ServiceOrderItems { get; set; } = new List<ServiceOrderItem>();
    }
}

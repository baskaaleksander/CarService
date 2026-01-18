using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarService.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa usługi jest wymagana")]
        [StringLength(100, ErrorMessage = "Nazwa usługi nie może przekraczać 100 znaków")]
        [Display(Name = "Nazwa usługi")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Opis nie może przekraczać 500 znaków")]
        [Display(Name = "Opis")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Cena jest wymagana")]
        [Range(0.01, 100000, ErrorMessage = "Cena musi być w zakresie od 0,01 do 100 000")]
        [DataType(DataType.Currency)]
        [Display(Name = "Cena")]
        public decimal Price { get; set; }

        [Display(Name = "Aktywna")]
        public bool IsActive { get; set; } = true;

        public ICollection<ServiceOrderItem> ServiceOrderItems { get; set; } = new List<ServiceOrderItem>();
    }
}

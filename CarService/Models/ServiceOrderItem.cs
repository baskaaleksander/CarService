using System.ComponentModel.DataAnnotations;

namespace CarService.Models
{
    public class ServiceOrderItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Zlecenie serwisowe jest wymagane")]
        public int ServiceOrderId { get; set; }

        public int? ServiceId { get; set; }

        public int? PartId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Ilość musi być co najmniej 1")]
        [Display(Name = "Ilość")]
        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public ServiceOrder? ServiceOrder { get; set; }

        public Service? Service { get; set; }

        public Part? Part { get; set; }
    }
}

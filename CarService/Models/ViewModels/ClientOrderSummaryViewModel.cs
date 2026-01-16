using CarService.Models.Enums;

namespace CarService.Models.ViewModels
{
    public class ClientOrderSummaryViewModel
    {
        public int Id { get; set; }
        public string VehicleInfo { get; set; } = string.Empty;
        public ServiceOrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalCost { get; set; }
    }
}

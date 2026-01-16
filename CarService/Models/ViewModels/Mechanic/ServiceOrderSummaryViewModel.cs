using CarService.Models.Enums;

namespace CarService.Models.ViewModels.Mechanic
{
    public class ServiceOrderSummaryViewModel
    {
        public int Id { get; set; }
        public string VehicleInfo { get; set; } = string.Empty;
        public ServiceOrderStatus Status { get; set; }
        public string StatusBadgeClass { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public decimal TotalCost { get; set; }
    }
}

using CarService.Models.Enums;

namespace CarService.Models.ViewModels
{
    public class VehicleDetailsViewModel
    {
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string VIN { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public List<ServiceHistoryItemViewModel> ServiceHistory { get; set; } = new();
    }

    public class ServiceHistoryItemViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public ServiceOrderStatus Status { get; set; }
        public decimal TotalCost { get; set; }
    }
}

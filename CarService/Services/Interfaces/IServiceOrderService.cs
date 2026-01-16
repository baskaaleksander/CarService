using System.Collections.Generic;
using System.Threading.Tasks;
using CarService.Models;
using CarService.Models.Enums;

namespace CarService.Services.Interfaces
{
    public interface IServiceOrderService
    {
        Task<IEnumerable<ServiceOrder>> GetAllAsync();
        Task<IEnumerable<ServiceOrder>> GetByClientAsync(string clientId);
        Task<IEnumerable<ServiceOrder>> GetByMechanicAsync(string mechanicId);
        Task<IEnumerable<ServiceOrder>> GetByStatusAsync(ServiceOrderStatus status);
        Task<ServiceOrder?> GetByIdAsync(int id);
        Task<ServiceOrder?> GetByIdWithDetailsAsync(int id);

        Task<ServiceOrder> CreateAsync(ServiceOrder order);
        Task AssignMechanicAsync(int orderId, string mechanicId);
        Task UpdateStatusAsync(int orderId, ServiceOrderStatus status);
        Task AddDiagnosticNotesAsync(int orderId, string notes);
        Task SetLaborHoursAsync(int orderId, decimal hours);

        Task AddServiceItemAsync(int orderId, int serviceId, int quantity);
        Task AddPartItemAsync(int orderId, int partId, int quantity);
        Task RemoveItemAsync(int itemId);

        Task<decimal> CalculateTotalCostAsync(int orderId);
        Task UpdateTotalCostAsync(int orderId);

        Task<bool> CanCompleteAsync(int orderId);
        Task<bool> BelongsToClientAsync(int orderId, string clientId);
        Task<bool> IsAssignedToMechanicAsync(int orderId, string mechanicId);
    }
}

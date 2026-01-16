using System.Collections.Generic;
using System.Threading.Tasks;
using CarService.Models;

namespace CarService.Services.Interfaces
{
    public interface IVehicleService
    {
        Task<IEnumerable<Vehicle>> GetByOwnerAsync(string ownerId);
        Task<Vehicle?> GetByIdAsync(int id);
        Task<Vehicle?> GetByIdForOwnerAsync(int id, string ownerId);
        Task<Vehicle?> GetByIdWithServiceHistoryAsync(int id, string ownerId);
        Task<Vehicle> CreateAsync(Vehicle vehicle);
        Task<Vehicle> UpdateAsync(Vehicle vehicle);
        Task DeleteAsync(int id);
        Task<bool> IsVinUniqueAsync(string vin, int? excludeId = null);
    }
}

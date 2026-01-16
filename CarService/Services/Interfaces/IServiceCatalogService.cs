using System.Collections.Generic;
using System.Threading.Tasks;
using CarService.Models;

namespace CarService.Services.Interfaces
{
    public interface IServiceCatalogService
    {
        Task<IEnumerable<Service>> GetAllAsync();
        Task<IEnumerable<Service>> GetActiveAsync();
        Task<Service?> GetByIdAsync(int id);
        Task<Service> CreateAsync(Service service);
        Task<Service> UpdateAsync(Service service);
        Task DeactivateAsync(int id);
        Task ActivateAsync(int id);
    }
}

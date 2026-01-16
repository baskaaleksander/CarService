using System.Collections.Generic;
using System.Threading.Tasks;
using CarService.Models;

namespace CarService.Services.Interfaces
{
    public interface IPartService
    {
        Task<IEnumerable<Part>> GetAllAsync();
        Task<IEnumerable<Part>> GetInStockAsync();
        Task<Part?> GetByIdAsync(int id);
        Task<Part> CreateAsync(Part part);
        Task<Part> UpdateAsync(Part part);
        Task<bool> UpdateStockAsync(int id, int quantityChange);
        Task<bool> HasSufficientStockAsync(int id, int requiredQuantity);
    }
}

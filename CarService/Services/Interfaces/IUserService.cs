using System.Collections.Generic;
using System.Threading.Tasks;
using CarService.Models;

namespace CarService.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<IEnumerable<ApplicationUser>> GetByRoleAsync(string role);
        Task<IEnumerable<ApplicationUser>> GetMechanicsAsync();
        Task<IEnumerable<ApplicationUser>> GetClientsAsync();
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<IList<string>> GetRolesAsync(string userId);
        Task ChangeRoleAsync(string userId, string newRole);
    }
}

using CarService.Data;
using CarService.Models;
using CarService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarService.Services
{
    public class ServiceCatalogService : IServiceCatalogService
    {
        private readonly ApplicationDbContext _context;

        public ServiceCatalogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Service>> GetAllAsync()
        {
            return await _context.Services.ToListAsync();
        }

        public async Task<IEnumerable<Service>> GetActiveAsync()
        {
            return await _context.Services
                .Where(s => s.IsActive)
                .ToListAsync();
        }

        public async Task<Service?> GetByIdAsync(int id)
        {
            return await _context.Services.FindAsync(id);
        }

        public async Task<Service> CreateAsync(Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return service;
        }

        public async Task<Service> UpdateAsync(Service service)
        {
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
            return service;
        }

        public async Task DeactivateAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                service.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ActivateAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                service.IsActive = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}

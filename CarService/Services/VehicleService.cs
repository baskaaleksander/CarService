using CarService.Data;
using CarService.Models;
using CarService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarService.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly ApplicationDbContext _context;

        public VehicleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vehicle>> GetByOwnerAsync(string ownerId)
        {
            return await _context.Vehicles
                .Where(v => v.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task<Vehicle?> GetByIdAsync(int id)
        {
            return await _context.Vehicles
                .Include(v => v.Owner)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Vehicle?> GetByIdForOwnerAsync(int id, string ownerId)
        {
            return await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == id && v.OwnerId == ownerId);
        }

        public async Task<Vehicle> CreateAsync(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task<Vehicle> UpdateAsync(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task DeleteAsync(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsVinUniqueAsync(string vin, int? excludeId = null)
        {
            return !await _context.Vehicles
                .AnyAsync(v => v.VIN == vin && (!excludeId.HasValue || v.Id != excludeId.Value));
        }
    }
}

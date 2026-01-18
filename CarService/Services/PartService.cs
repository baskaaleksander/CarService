using CarService.Data;
using CarService.Models;
using CarService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarService.Services
{
    public class PartService : IPartService
    {
        private readonly ApplicationDbContext _context;

        public PartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Part>> GetAllAsync()
        {
            return await _context.Parts.ToListAsync();
        }

        public async Task<IEnumerable<Part>> GetInStockAsync()
        {
            return await _context.Parts
                .Where(p => p.StockQuantity > 0)
                .ToListAsync();
        }

        public async Task<Part?> GetByIdAsync(int id)
        {
            return await _context.Parts.FindAsync(id);
        }

        public async Task<Part> CreateAsync(Part part)
        {
            _context.Parts.Add(part);
            await _context.SaveChangesAsync();
            return part;
        }

        public async Task<Part> UpdateAsync(Part part)
        {
            _context.Parts.Update(part);
            await _context.SaveChangesAsync();
            return part;
        }

        public async Task<bool> UpdateStockAsync(int id, int quantityChange)
        {
            var part = await _context.Parts.FindAsync(id);
            if (part == null) return false;

            var newQuantity = part.StockQuantity + quantityChange;
            if (newQuantity < 0)
            {
                throw new InvalidOperationException($"Niewystarczająca ilość na stanie dla części '{part.Name}'. Dostępne: {part.StockQuantity}, żądana zmiana: {quantityChange}");
            }

            part.StockQuantity = newQuantity;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasSufficientStockAsync(int id, int requiredQuantity)
        {
            var part = await _context.Parts.FindAsync(id);
            return part != null && part.StockQuantity >= requiredQuantity;
        }
    }
}

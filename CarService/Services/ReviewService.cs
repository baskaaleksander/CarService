using CarService.Data;
using CarService.Models;
using CarService.Models.Enums;
using CarService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarService.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Review?> GetByOrderIdAsync(int orderId)
        {
            return await _context.Reviews
                .Include(r => r.ServiceOrder)
                .FirstOrDefaultAsync(r => r.ServiceOrderId == orderId);
        }

        public async Task<IEnumerable<Review>> GetRecentAsync(int count = 10)
        {
            return await _context.Reviews
                .Include(r => r.ServiceOrder)
                    .ThenInclude(o => o!.Client)
                .OrderByDescending(r => r.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Review> CreateAsync(Review review)
        {
            var order = await _context.ServiceOrders.FindAsync(review.ServiceOrderId);
            if (order == null)
                throw new InvalidOperationException("Nie znaleziono zlecenia");

            if (order.Status != ServiceOrderStatus.Completed)
                throw new InvalidOperationException("Można oceniać tylko zakończone zlecenia");

            var existingReview = await _context.Reviews
                .AnyAsync(r => r.ServiceOrderId == review.ServiceOrderId);
            if (existingReview)
                throw new InvalidOperationException("Zlecenie ma już opinię");

            if (review.Rating < 1 || review.Rating > 5)
                throw new InvalidOperationException("Ocena musi być w zakresie od 1 do 5");

            review.CreatedAt = DateTime.UtcNow;
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<bool> CanReviewOrderAsync(int orderId, string clientId)
        {
            var order = await _context.ServiceOrders
                .Include(o => o.Review)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return false;

            return order.ClientId == clientId 
                   && order.Status == ServiceOrderStatus.Completed 
                   && order.Review == null;
        }

        public async Task<double> GetAverageRatingAsync()
        {
            if (!await _context.Reviews.AnyAsync())
                return 0;

            return await _context.Reviews.AverageAsync(r => r.Rating);
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using CarService.Models;

namespace CarService.Services.Interfaces
{
    public interface IReviewService
    {
        Task<Review?> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<Review>> GetRecentAsync(int count = 10);
        Task<Review> CreateAsync(Review review);
        Task<bool> CanReviewOrderAsync(int orderId, string clientId);
        Task<double> GetAverageRatingAsync();
    }
}

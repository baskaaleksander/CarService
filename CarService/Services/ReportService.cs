using System.Globalization;
using CarService.Data;
using CarService.Models.Enums;
using CarService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarService.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MonthlyRevenueDto>> GetMonthlyRevenueAsync(int year)
        {
            return await _context.ServiceOrders
                .Where(o => o.CompletedAt.HasValue && o.CompletedAt.Value.Year == year)
                .GroupBy(o => o.CompletedAt!.Value.Month)
                .Select(g => new MonthlyRevenueDto(
                    g.Key,
                    CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                    g.Sum(o => o.TotalCost),
                    g.Count()
                ))
                .ToListAsync();
        }

        public async Task<IEnumerable<PopularServiceDto>> GetPopularServicesAsync(int topN = 10)
        {
            return await _context.ServiceOrderItems
                .Where(i => i.ServiceId.HasValue)
                .GroupBy(i => new { i.ServiceId, i.Service!.Name })
                .Select(g => new PopularServiceDto(
                    g.Key.ServiceId!.Value,
                    g.Key.Name,
                    g.Count(),
                    g.Sum(i => i.Quantity * i.UnitPrice)
                ))
                .OrderByDescending(x => x.UsageCount)
                .Take(topN)
                .ToListAsync();
        }

        public async Task<IEnumerable<MechanicEfficiencyDto>> GetMechanicEfficiencyAsync()
        {
            return await _context.ServiceOrders
                .Where(o => o.MechanicId != null && o.Status == ServiceOrderStatus.Completed)
                .GroupBy(o => new { o.MechanicId, o.Mechanic!.FirstName, o.Mechanic.LastName })
                .Select(g => new MechanicEfficiencyDto(
                    g.Key.MechanicId!,
                    $"{g.Key.FirstName} {g.Key.LastName}",
                    g.Count(),
                    g.Average(o => o.LaborHours)
                ))
                .ToListAsync();
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var totalOrders = await _context.ServiceOrders.CountAsync();
            
            var pendingOrders = await _context.ServiceOrders
                .CountAsync(o => o.Status == ServiceOrderStatus.Pending);
            
            var activeMechanics = await _context.ServiceOrders
                .Where(o => o.MechanicId != null && 
                           (o.Status == ServiceOrderStatus.Accepted || o.Status == ServiceOrderStatus.InProgress))
                .Select(o => o.MechanicId)
                .Distinct()
                .CountAsync();

            var now = DateTime.UtcNow;
            var monthRevenue = await _context.ServiceOrders
                .Where(o => o.CompletedAt.HasValue && 
                           o.CompletedAt.Value.Year == now.Year && 
                           o.CompletedAt.Value.Month == now.Month)
                .SumAsync(o => o.TotalCost);

            return new DashboardStatsDto(totalOrders, pendingOrders, activeMechanics, monthRevenue);
        }
    }
}

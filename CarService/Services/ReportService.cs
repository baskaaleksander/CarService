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
            var orders = await _context.ServiceOrders
                .Where(o => o.CompletedAt.HasValue && o.CompletedAt.Value.Year == year)
                .Select(o => new { Month = o.CompletedAt!.Value.Month, o.TotalCost })
                .ToListAsync();

            return orders
                .GroupBy(o => o.Month)
                .Select(g => new MonthlyRevenueDto(
                    g.Key,
                    CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                    g.Sum(o => o.TotalCost),
                    g.Count()
                ))
                .OrderBy(x => x.Month)
                .ToList();
        }

        public async Task<IEnumerable<PopularServiceDto>> GetPopularServicesAsync(int topN = 10)
        {
            var items = await _context.ServiceOrderItems
                .Where(i => i.ServiceId.HasValue)
                .Select(i => new { i.ServiceId, ServiceName = i.Service!.Name, i.Quantity, i.UnitPrice })
                .ToListAsync();

            return items
                .GroupBy(i => new { i.ServiceId, i.ServiceName })
                .Select(g => new PopularServiceDto(
                    g.Key.ServiceId!.Value,
                    g.Key.ServiceName,
                    g.Count(),
                    g.Sum(i => i.Quantity * i.UnitPrice)
                ))
                .OrderByDescending(x => x.UsageCount)
                .Take(topN)
                .ToList();
        }

        public async Task<IEnumerable<MechanicEfficiencyDto>> GetMechanicEfficiencyAsync()
        {
            var orders = await _context.ServiceOrders
                .Where(o => o.MechanicId != null && o.Status == ServiceOrderStatus.Completed)
                .Select(o => new { o.MechanicId, o.Mechanic!.FirstName, o.Mechanic.LastName, o.LaborHours })
                .ToListAsync();

            return orders
                .GroupBy(o => new { o.MechanicId, o.FirstName, o.LastName })
                .Select(g => new MechanicEfficiencyDto(
                    g.Key.MechanicId!,
                    $"{g.Key.FirstName} {g.Key.LastName}",
                    g.Count(),
                    g.Any() ? g.Average(o => o.LaborHours) : 0
                ))
                .ToList();
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
            var monthOrderTotals = await _context.ServiceOrders
                .Where(o => o.CompletedAt.HasValue && 
                           o.CompletedAt.Value.Year == now.Year && 
                           o.CompletedAt.Value.Month == now.Month)
                .Select(o => o.TotalCost)
                .ToListAsync();
            
            var monthRevenue = monthOrderTotals.Sum();

            return new DashboardStatsDto(totalOrders, pendingOrders, activeMechanics, monthRevenue);
        }
    }
}

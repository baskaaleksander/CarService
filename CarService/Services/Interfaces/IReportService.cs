using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarService.Services.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<MonthlyRevenueDto>> GetMonthlyRevenueAsync(int year);
        Task<IEnumerable<PopularServiceDto>> GetPopularServicesAsync(int topN = 10);
        Task<IEnumerable<MechanicEfficiencyDto>> GetMechanicEfficiencyAsync();
        Task<DashboardStatsDto> GetDashboardStatsAsync();
    }

    public record MonthlyRevenueDto(int Month, string MonthName, decimal Revenue, int OrderCount);
    public record PopularServiceDto(int ServiceId, string ServiceName, int UsageCount, decimal TotalRevenue);
    public record MechanicEfficiencyDto(string MechanicId, string MechanicName, int CompletedOrders, decimal AvgCompletionHours);
    public record DashboardStatsDto(int TotalOrders, int PendingOrders, int ActiveMechanics, decimal MonthRevenue);
}

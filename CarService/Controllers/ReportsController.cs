using CarService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<IActionResult> Index()
        {
            var stats = await _reportService.GetDashboardStatsAsync();
            return View(stats);
        }

        public async Task<IActionResult> Revenue(int? year)
        {
            year ??= DateTime.UtcNow.Year;
            var revenue = await _reportService.GetMonthlyRevenueAsync(year.Value);
            ViewBag.Year = year;
            return View(revenue);
        }

        public async Task<IActionResult> PopularServices()
        {
            var services = await _reportService.GetPopularServicesAsync();
            return View(services);
        }

        public async Task<IActionResult> MechanicEfficiency()
        {
            var efficiency = await _reportService.GetMechanicEfficiencyAsync();
            return View(efficiency);
        }
    }
}

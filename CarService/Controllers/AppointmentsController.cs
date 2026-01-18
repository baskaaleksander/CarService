using CarService.Models;
using CarService.Models.Enums;
using CarService.Models.ViewModels;
using CarService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarService.Controllers
{
    [Authorize(Policy = "ClientOnly")]
    public class AppointmentsController : Controller
    {
        private readonly IServiceOrderService _orderService;
        private readonly IVehicleService _vehicleService;
        private readonly IServiceCatalogService _catalogService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsController(
            IServiceOrderService orderService,
            IVehicleService vehicleService,
            IServiceCatalogService catalogService,
            UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _vehicleService = vehicleService;
            _catalogService = catalogService;
            _userManager = userManager;
        }

        // GET: Appointments/Book
        public async Task<IActionResult> Book()
        {
            var userId = _userManager.GetUserId(User)!;
            var vehicles = await _vehicleService.GetByOwnerAsync(userId);

            if (!vehicles.Any())
            {
                TempData["Error"] = "Najpierw dodaj pojazd, aby umówić wizytę.";
                return RedirectToAction("Create", "Vehicles");
            }

            var model = new AppointmentBookingViewModel
            {
                AvailableVehicles = vehicles.Select(v => new SelectListItem
                {
                    Value = v.Id.ToString(),
                    Text = $"{v.Brand} {v.Model} ({v.RegistrationNumber})"
                }),
                AvailableServices = (await _catalogService.GetActiveAsync()).Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.Name} - {s.Price:C}"
                })
            };

            return View(model);
        }

        // POST: Appointments/Book
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(AppointmentBookingViewModel model)
        {
            var userId = _userManager.GetUserId(User)!;

            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns
                var vehicles = await _vehicleService.GetByOwnerAsync(userId);
                model.AvailableVehicles = vehicles.Select(v => new SelectListItem
                {
                    Value = v.Id.ToString(),
                    Text = $"{v.Brand} {v.Model} ({v.RegistrationNumber})"
                });
                model.AvailableServices = (await _catalogService.GetActiveAsync()).Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.Name} - {s.Price:C}"
                });
                return View(model);
            }

            // Verify vehicle belongs to user
            var vehicle = await _vehicleService.GetByIdForOwnerAsync(model.VehicleId, userId);
            if (vehicle == null)
            {
                return Forbid();
            }

            var order = new ServiceOrder
            {
                VehicleId = model.VehicleId,
                ClientId = userId,
                Status = ServiceOrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                DiagnosticNotes = model.Description
            };

            var createdOrder = await _orderService.CreateAsync(order);

            // Add selected services if any
            if (model.SelectedServiceIds != null)
            {
                foreach (var serviceId in model.SelectedServiceIds)
                {
                    await _orderService.AddServiceItemAsync(createdOrder.Id, serviceId, 1);
                }
            }

            TempData["Success"] = "Wizyta została zarezerwowana pomyślnie.";
            return RedirectToAction("Details", "MyOrders", new { id = createdOrder.Id });
        }
    }
}

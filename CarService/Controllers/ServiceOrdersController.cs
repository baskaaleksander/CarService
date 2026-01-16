using CarService.Models;
using CarService.Models.Enums;
using CarService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarService.Controllers
{
    [Authorize]
    public class ServiceOrdersController : Controller
    {
        private readonly IServiceOrderService _orderService;
        private readonly IVehicleService _vehicleService;
        private readonly IServiceCatalogService _serviceCatalogService;
        private readonly IPartService _partService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ServiceOrdersController(
            IServiceOrderService orderService,
            IVehicleService vehicleService,
            IServiceCatalogService serviceCatalogService,
            IPartService partService,
            IUserService userService,
            UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _vehicleService = vehicleService;
            _serviceCatalogService = serviceCatalogService;
            _partService = partService;
            _userService = userService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;

            if (User.IsInRole("Admin"))
            {
                var orders = await _orderService.GetAllAsync();
                return View(orders);
            }
            else if (User.IsInRole("Mechanic"))
            {
                var orders = await _orderService.GetByMechanicAsync(userId);
                return View(orders);
            }
            else
            {
                var orders = await _orderService.GetByClientAsync(userId);
                return View(orders);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetByIdWithDetailsAsync(id);
            if (order == null) return NotFound();

            var userId = _userManager.GetUserId(User)!;
            if (!User.IsInRole("Admin") && order.ClientId != userId && order.MechanicId != userId)
                return Forbid();

            return View(order);
        }

        [Authorize(Policy = "ClientOnly")]
        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User)!;
            var vehicles = await _vehicleService.GetByOwnerAsync(userId);
            ViewBag.Vehicles = new SelectList(vehicles, "Id", "RegistrationNumber");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "ClientOnly")]
        public async Task<IActionResult> Create(ServiceOrder order)
        {
            var userId = _userManager.GetUserId(User)!;

            var vehicle = await _vehicleService.GetByIdForOwnerAsync(order.VehicleId, userId);
            if (vehicle == null)
            {
                ModelState.AddModelError("VehicleId", "Invalid vehicle selected.");
            }

            if (!ModelState.IsValid)
            {
                var vehicles = await _vehicleService.GetByOwnerAsync(userId);
                ViewBag.Vehicles = new SelectList(vehicles, "Id", "RegistrationNumber");
                return View(order);
            }

            order.ClientId = userId;
            await _orderService.CreateAsync(order);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Assign(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            var mechanics = await _userService.GetMechanicsAsync();
            ViewBag.Mechanics = new SelectList(mechanics, "Id", "Email");
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Assign(int id, string mechanicId)
        {
            await _orderService.AssignMechanicAsync(id, mechanicId);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "MechanicOrAdmin")]
        public async Task<IActionResult> UpdateStatus(int id, ServiceOrderStatus status)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            var userId = _userManager.GetUserId(User)!;
            if (!User.IsInRole("Admin") && order.MechanicId != userId)
                return Forbid();

            await _orderService.UpdateStatusAsync(id, status);
            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize(Policy = "MechanicOrAdmin")]
        public async Task<IActionResult> AddItem(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            var services = await _serviceCatalogService.GetActiveAsync();
            var parts = await _partService.GetInStockAsync();
            ViewBag.Services = new SelectList(services, "Id", "Name");
            ViewBag.Parts = new SelectList(parts, "Id", "Name");
            ViewBag.OrderId = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "MechanicOrAdmin")]
        public async Task<IActionResult> AddServiceItem(int orderId, int serviceId, int quantity)
        {
            await _orderService.AddServiceItemAsync(orderId, serviceId, quantity);
            return RedirectToAction(nameof(Details), new { id = orderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "MechanicOrAdmin")]
        public async Task<IActionResult> AddPartItem(int orderId, int partId, int quantity)
        {
            await _orderService.AddPartItemAsync(orderId, partId, quantity);
            return RedirectToAction(nameof(Details), new { id = orderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "MechanicOrAdmin")]
        public async Task<IActionResult> RemoveItem(int id, int itemId)
        {
            await _orderService.RemoveItemAsync(itemId);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "MechanicOrAdmin")]
        public async Task<IActionResult> UpdateDiagnostics(int id, string diagnosticNotes, decimal laborHours)
        {
            await _orderService.AddDiagnosticNotesAsync(id, diagnosticNotes);
            await _orderService.SetLaborHoursAsync(id, laborHours);
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Invoice(int id)
        {
            var order = await _orderService.GetByIdWithDetailsAsync(id);
            if (order == null) return NotFound();

            var userId = _userManager.GetUserId(User)!;
            if (!User.IsInRole("Admin") && order.ClientId != userId && order.MechanicId != userId)
                return Forbid();

            if (order.Status != ServiceOrderStatus.Completed)
            {
                TempData["Error"] = "Invoice can only be generated for completed orders.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(order);
        }
    }
}

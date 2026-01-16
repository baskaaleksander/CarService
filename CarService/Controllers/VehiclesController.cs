using CarService.Models;
using CarService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Controllers
{
    [Authorize(Policy = "ClientOnly")]
    public class VehiclesController : Controller
    {
        private readonly IVehicleService _vehicleService;
        private readonly UserManager<ApplicationUser> _userManager;

        public VehiclesController(IVehicleService vehicleService, UserManager<ApplicationUser> userManager)
        {
            _vehicleService = vehicleService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var vehicles = await _vehicleService.GetByOwnerAsync(userId);
            return View(vehicles);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vehicle vehicle)
        {
            var userId = _userManager.GetUserId(User)!;
            
            if (!await _vehicleService.IsVinUniqueAsync(vehicle.VIN))
            {
                ModelState.AddModelError("VIN", "This VIN is already registered.");
            }

            if (!ModelState.IsValid) return View(vehicle);

            vehicle.OwnerId = userId;
            await _vehicleService.CreateAsync(vehicle);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var vehicle = await _vehicleService.GetByIdForOwnerAsync(id, userId);
            if (vehicle == null) return NotFound();
            return View(vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vehicle vehicle)
        {
            if (id != vehicle.Id) return BadRequest();

            var userId = _userManager.GetUserId(User)!;
            var existing = await _vehicleService.GetByIdForOwnerAsync(id, userId);
            if (existing == null) return NotFound();

            if (!await _vehicleService.IsVinUniqueAsync(vehicle.VIN, id))
            {
                ModelState.AddModelError("VIN", "This VIN is already registered.");
            }

            if (!ModelState.IsValid) return View(vehicle);

            existing.Brand = vehicle.Brand;
            existing.Model = vehicle.Model;
            existing.VIN = vehicle.VIN;
            existing.RegistrationNumber = vehicle.RegistrationNumber;

            await _vehicleService.UpdateAsync(existing);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var vehicle = await _vehicleService.GetByIdForOwnerAsync(id, userId);
            if (vehicle == null) return NotFound();

            await _vehicleService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

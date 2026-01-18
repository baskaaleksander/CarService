using CarService.Models;
using CarService.Models.ViewModels;
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

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var vehicle = await _vehicleService.GetByIdWithServiceHistoryAsync(id, userId);
            if (vehicle == null)
                return NotFound();

            var viewModel = new VehicleDetailsViewModel
            {
                Id = vehicle.Id,
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                VIN = vehicle.VIN,
                RegistrationNumber = vehicle.RegistrationNumber,
                ServiceHistory = vehicle.ServiceOrders
                    .OrderByDescending(so => so.CreatedAt)
                    .Select(so => new ServiceHistoryItemViewModel
                    {
                        Id = so.Id,
                        CreatedAt = so.CreatedAt,
                        Status = so.Status,
                        TotalCost = so.Items.Sum(i => i.Quantity * i.UnitPrice)
                    }).ToList()
            };

            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View(new VehicleCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VehicleCreateViewModel viewModel)
        {
            var userId = _userManager.GetUserId(User)!;
            
            if (!await _vehicleService.IsVinUniqueAsync(viewModel.VIN))
            {
                ModelState.AddModelError("VIN", "Ten numer VIN jest już zarejestrowany.");
            }

            if (!ModelState.IsValid) return View(viewModel);

            var vehicle = new Vehicle
            {
                Brand = viewModel.Brand,
                Model = viewModel.Model,
                VIN = viewModel.VIN.ToUpper(),
                RegistrationNumber = viewModel.RegistrationNumber.ToUpper(),
                OwnerId = userId
            };

            await _vehicleService.CreateAsync(vehicle);
            TempData["Success"] = "Pojazd został dodany pomyślnie.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var vehicle = await _vehicleService.GetByIdForOwnerAsync(id, userId);
            if (vehicle == null) return NotFound();

            var viewModel = new VehicleEditViewModel
            {
                Id = vehicle.Id,
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                VIN = vehicle.VIN,
                RegistrationNumber = vehicle.RegistrationNumber
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VehicleEditViewModel viewModel)
        {
            if (id != viewModel.Id) return BadRequest();

            var userId = _userManager.GetUserId(User)!;
            var existing = await _vehicleService.GetByIdForOwnerAsync(id, userId);
            if (existing == null) return NotFound();

            if (!await _vehicleService.IsVinUniqueAsync(viewModel.VIN, id))
            {
                ModelState.AddModelError("VIN", "Ten numer VIN jest już zarejestrowany.");
            }

            if (!ModelState.IsValid) return View(viewModel);

            existing.Brand = viewModel.Brand;
            existing.Model = viewModel.Model;
            existing.VIN = viewModel.VIN.ToUpper();
            existing.RegistrationNumber = viewModel.RegistrationNumber.ToUpper();

            await _vehicleService.UpdateAsync(existing);
            TempData["Success"] = "Pojazd został zaktualizowany pomyślnie.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var vehicle = await _vehicleService.GetByIdForOwnerAsync(id, userId);
            if (vehicle == null) return NotFound();

            return View(vehicle);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var vehicle = await _vehicleService.GetByIdForOwnerAsync(id, userId);
            if (vehicle == null) return NotFound();

            await _vehicleService.DeleteAsync(id);
            TempData["Success"] = "Pojazd został usunięty pomyślnie.";
            return RedirectToAction(nameof(Index));
        }
    }
}

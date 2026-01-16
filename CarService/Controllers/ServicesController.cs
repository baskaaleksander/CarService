using CarService.Models;
using CarService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class ServicesController : Controller
    {
        private readonly IServiceCatalogService _serviceCatalogService;

        public ServicesController(IServiceCatalogService serviceCatalogService)
        {
            _serviceCatalogService = serviceCatalogService;
        }

        public async Task<IActionResult> Index()
        {
            var services = await _serviceCatalogService.GetAllAsync();
            return View(services);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service service)
        {
            if (!ModelState.IsValid) return View(service);

            await _serviceCatalogService.CreateAsync(service);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var service = await _serviceCatalogService.GetByIdAsync(id);
            if (service == null) return NotFound();
            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Service service)
        {
            if (id != service.Id) return BadRequest();
            if (!ModelState.IsValid) return View(service);

            await _serviceCatalogService.UpdateAsync(service);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var service = await _serviceCatalogService.GetByIdAsync(id);
            if (service == null) return NotFound();

            if (service.IsActive)
                await _serviceCatalogService.DeactivateAsync(id);
            else
                await _serviceCatalogService.ActivateAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}

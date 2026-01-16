using CarService.Models;
using CarService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class PartsController : Controller
    {
        private readonly IPartService _partService;

        public PartsController(IPartService partService)
        {
            _partService = partService;
        }

        public async Task<IActionResult> Index()
        {
            var parts = await _partService.GetAllAsync();
            return View(parts);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Part part)
        {
            if (!ModelState.IsValid) return View(part);

            await _partService.CreateAsync(part);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var part = await _partService.GetByIdAsync(id);
            if (part == null) return NotFound();
            return View(part);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Part part)
        {
            if (id != part.Id) return BadRequest();
            if (!ModelState.IsValid) return View(part);

            await _partService.UpdateAsync(part);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStock(int id, int quantityChange)
        {
            await _partService.UpdateStockAsync(id, quantityChange);
            return RedirectToAction(nameof(Index));
        }
    }
}

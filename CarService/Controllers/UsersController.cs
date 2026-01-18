using CarService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllAsync();
            var usersWithRoles = new List<(Models.ApplicationUser User, IList<string> Roles)>();
            
            foreach (var user in users)
            {
                var roles = await _userService.GetRolesAsync(user.Id);
                usersWithRoles.Add((user, roles));
            }

            return View(usersWithRoles);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userService.GetRolesAsync(id);
            return View((user, roles));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string userId, string newRole)
        {
            await _userService.ChangeRoleAsync(userId, newRole);
            TempData["Success"] = "Rola użytkownika została zaktualizowana pomyślnie.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string userId, string newRole)
        {
            await _userService.ChangeRoleAsync(userId, newRole);
            return RedirectToAction(nameof(Index));
        }
    }
}

using CarService.Models;
using CarService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IServiceOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(
            IReviewService reviewService,
            IServiceOrderService orderService,
            UserManager<ApplicationUser> userManager)
        {
            _reviewService = reviewService;
            _orderService = orderService;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var reviews = await _reviewService.GetRecentAsync(20);
            ViewBag.AverageRating = await _reviewService.GetAverageRatingAsync();
            return View(reviews);
        }

        [Authorize(Policy = "ClientOnly")]
        public async Task<IActionResult> Create(int orderId)
        {
            var userId = _userManager.GetUserId(User)!;
            
            if (!await _reviewService.CanReviewOrderAsync(orderId, userId))
                return BadRequest("Nie można ocenić tego zlecenia.");

            ViewBag.OrderId = orderId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "ClientOnly")]
        public async Task<IActionResult> Create(Review review)
        {
            var userId = _userManager.GetUserId(User)!;
            
            if (!await _reviewService.CanReviewOrderAsync(review.ServiceOrderId, userId))
                return BadRequest("Nie można ocenić tego zlecenia.");

            if (!ModelState.IsValid)
            {
                ViewBag.OrderId = review.ServiceOrderId;
                return View(review);
            }

            await _reviewService.CreateAsync(review);
            return RedirectToAction("Details", "ServiceOrders", new { id = review.ServiceOrderId });
        }
    }
}

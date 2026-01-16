using CarService.Models;
using CarService.Models.ViewModels;
using CarService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Controllers
{
    [Authorize(Policy = "ClientOnly")]
    public class MyOrdersController : Controller
    {
        private readonly IServiceOrderService _orderService;
        private readonly IReviewService _reviewService;
        private readonly UserManager<ApplicationUser> _userManager;

        public MyOrdersController(
            IServiceOrderService orderService,
            IReviewService reviewService,
            UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _reviewService = reviewService;
            _userManager = userManager;
        }

        // GET: MyOrders
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var orders = await _orderService.GetByClientAsync(userId);
            return View(orders);
        }

        // GET: MyOrders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User)!;

            if (!await _orderService.BelongsToClientAsync(id, userId))
                return Forbid();

            var order = await _orderService.GetByIdWithDetailsAsync(id);
            if (order == null)
                return NotFound();

            var canReview = await _reviewService.CanReviewOrderAsync(id, userId);

            var items = order.Items.Select(i => new ServiceOrderItemViewModel
            {
                Name = i.Service?.Name ?? i.Part?.Name ?? "Unknown",
                Type = i.ServiceId.HasValue ? "Service" : "Part",
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.Quantity * i.UnitPrice
            }).ToList();

            var viewModel = new ServiceOrderDetailsViewModel
            {
                Id = order.Id,
                VehicleInfo = $"{order.Vehicle?.Brand} {order.Vehicle?.Model} ({order.Vehicle?.RegistrationNumber})",
                Status = order.Status.ToString(),
                CreatedAt = order.CreatedAt,
                CompletedAt = order.CompletedAt,
                TotalCost = items.Sum(i => i.TotalPrice),
                DiagnosticNotes = order.DiagnosticNotes,
                MechanicName = order.Mechanic != null ? $"{order.Mechanic.FirstName} {order.Mechanic.LastName}" : null,
                CanReview = canReview,
                Items = items
            };

            if (order.Review != null)
            {
                viewModel.Review = new ReviewViewModel
                {
                    Rating = order.Review.Rating,
                    Comment = order.Review.Comment,
                    CreatedAt = order.Review.CreatedAt
                };
            }

            return View(viewModel);
        }

        // GET: MyOrders/AddReview/5
        public async Task<IActionResult> AddReview(int id)
        {
            var userId = _userManager.GetUserId(User)!;

            if (!await _reviewService.CanReviewOrderAsync(id, userId))
            {
                TempData["Error"] = "Cannot add review to this order.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var order = await _orderService.GetByIdWithDetailsAsync(id);
            if (order == null)
                return NotFound();

            var model = new ReviewCreateViewModel
            {
                ServiceOrderId = id,
                VehicleInfo = $"{order.Vehicle?.Brand} {order.Vehicle?.Model}"
            };

            return View(model);
        }

        // POST: MyOrders/AddReview/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int id, ReviewCreateViewModel model)
        {
            var userId = _userManager.GetUserId(User)!;

            if (!await _reviewService.CanReviewOrderAsync(id, userId))
                return Forbid();

            if (!ModelState.IsValid)
                return View(model);

            var review = new Review
            {
                ServiceOrderId = id,
                Rating = model.Rating,
                Comment = model.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _reviewService.CreateAsync(review);
            TempData["Success"] = "Thank you for your review!";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}

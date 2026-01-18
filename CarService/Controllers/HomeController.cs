using System.Collections.Generic;
using System.Diagnostics;
using CarService.Models;
using CarService.Models.ViewModels;
using CarService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IReviewService _reviewService;

    public HomeController(ILogger<HomeController> logger, IReviewService reviewService)
    {
        _logger = logger;
        _reviewService = reviewService;
    }

    public async Task<IActionResult> Index()
    {
        var recentReviews = await _reviewService.GetRecentAsync(6);
        var averageRating = await _reviewService.GetAverageRatingAsync();
        var testimonials = new List<HomeTestimonialViewModel>();

        foreach (var review in recentReviews)
        {
            var clientName = "Klient";
            if (review.ServiceOrder?.Client != null)
            {
                var combinedName = $"{review.ServiceOrder.Client.FirstName} {review.ServiceOrder.Client.LastName}".Trim();
                if (!string.IsNullOrWhiteSpace(combinedName))
                {
                    clientName = combinedName;
                }
            }

            testimonials.Add(new HomeTestimonialViewModel
            {
                Rating = review.Rating,
                Comment = review.Comment,
                ClientName = clientName,
                CreatedAt = review.CreatedAt
            });
        }

        var model = new HomeIndexViewModel
        {
            AverageRating = averageRating,
            Testimonials = testimonials
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
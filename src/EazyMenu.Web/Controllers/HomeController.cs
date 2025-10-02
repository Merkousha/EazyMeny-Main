using System.Diagnostics;
using System.Security.Claims;
using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using EazyMenu.Web.Models;

namespace EazyMenu.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IRepository<Restaurant> _restaurantRepository;

    public HomeController(
        ILogger<HomeController> logger,
        IRepository<Restaurant> restaurantRepository)
    {
        _logger = logger;
        _restaurantRepository = restaurantRepository;
    }

    public async Task<IActionResult> Index()
    {
        // اگر کاربر لاگین کرده و RestaurantOwner است، Slug رستوران را بگیر
        if (User.Identity?.IsAuthenticated == true && User.IsInRole("RestaurantOwner"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var ownerGuid))
            {
                var restaurants = await _restaurantRepository.FindAsync(
                    r => r.OwnerId == ownerGuid && !r.IsDeleted,
                    CancellationToken.None);
                
                var restaurant = restaurants.FirstOrDefault();
                if (restaurant != null)
                {
                    ViewBag.RestaurantSlug = restaurant.Slug;
                }
            }
        }

        return View();
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

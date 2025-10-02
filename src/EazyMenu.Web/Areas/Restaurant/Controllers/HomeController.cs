using System.Security.Claims;
using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EazyMenu.Web.Areas.Restaurant.Controllers;

/// <summary>
/// داشبورد صاحب رستوران
/// </summary>
[Area("Restaurant")]
[Authorize(Roles = "RestaurantOwner")]
public class HomeController : BaseRestaurantController
{
    private readonly IRepository<Domain.Entities.Restaurant> _restaurantRepository;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IMediator mediator,
        IRepository<Domain.Entities.Restaurant> restaurantRepository,
        ILogger<HomeController> logger) 
        : base(mediator)
    {
        _restaurantRepository = restaurantRepository;
        _logger = logger;
    }

    /// <summary>
    /// صفحه اصلی داشبورد صاحب رستوران
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var ownerGuid))
            {
                TempData["Error"] = "خطا در احراز هویت. لطفاً مجدداً وارد شوید";
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            // دریافت رستوران‌های کاربر
            var restaurants = await _restaurantRepository.FindAsync(
                r => r.OwnerId == ownerGuid && !r.IsDeleted,
                CancellationToken.None);

            var restaurant = restaurants.FirstOrDefault();

            // ارسال Slug برای استفاده در View
            if (restaurant != null)
            {
                ViewBag.RestaurantSlug = restaurant.Slug;
                ViewBag.RestaurantName = restaurant.Name;
                ViewBag.RestaurantId = restaurant.Id;
                ViewBag.IsWebsitePublished = restaurant.IsWebsitePublished;
            }
            else
            {
                // اگر رستورانی ندارد، به صفحه ایجاد رستوران هدایت می‌شود
                TempData["Info"] = "لطفاً ابتدا رستوران خود را ایجاد کنید";
                // TODO: Redirect to Restaurant/Create when that page exists
            }

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading restaurant dashboard");
            TempData["Error"] = "خطا در بارگذاری داشبورد";
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}

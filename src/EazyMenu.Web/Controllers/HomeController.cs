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
        // اگر کاربر لاگین کرده است
        if (User.Identity?.IsAuthenticated == true)
        {
            // Admin به داشبورد Admin می‌رود
            if (User.IsInRole("Admin"))
            {
                return Redirect("/Admin/Home/Index");
            }
            
            // RestaurantOwner به داشبورد Restaurant می‌رود
            if (User.IsInRole("RestaurantOwner"))
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
                        // Redirect به داشبورد اختصاصی RestaurantOwner
                        return Redirect("/Restaurant/Home/Index");
                    }
                }
            }
        }

        // کاربران Guest به صفحه اصلی می‌روند
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// درباره ما - صفحه معرفی پلتفرم
    /// </summary>
    public IActionResult About()
    {
        return View();
    }

    /// <summary>
    /// قیمت‌ها - صفحه پلن‌های اشتراک
    /// </summary>
    public IActionResult Pricing()
    {
        return View();
    }

    /// <summary>
    /// امکانات - صفحه ویژگی‌های سیستم
    /// </summary>
    public IActionResult Features()
    {
        return View();
    }

    /// <summary>
    /// تماس با ما - فرم ارتباط با پشتیبانی
    /// </summary>
    public IActionResult Contact()
    {
        return View();
    }

    /// <summary>
    /// سوالات متداول
    /// </summary>
    public IActionResult FAQ()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

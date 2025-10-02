using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EazyMenu.Web.Areas.Restaurant.Controllers;

/// <summary>
/// کنترلر گزارش‌ها و آنالیز
/// </summary>
[Area("Restaurant")]
[Authorize(Roles = "RestaurantOwner,Admin")]
public class AnalyticsController : BaseRestaurantController
{
    public AnalyticsController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// صفحه داشبورد آنالیز و گزارش‌ها
    /// Route: /Restaurant/Analytics/Index
    /// </summary>
    public IActionResult Index()
    {
        // RestaurantId, RestaurantSlug, RestaurantName از BaseRestaurantController تزریق شده‌اند
        return View();
    }

    /// <summary>
    /// گزارش فروش
    /// </summary>
    public IActionResult Sales()
    {
        return View();
    }

    /// <summary>
    /// گزارش محصولات پرفروش
    /// </summary>
    public IActionResult TopProducts()
    {
        return View();
    }

    /// <summary>
    /// گزارش مشتریان
    /// </summary>
    public IActionResult Customers()
    {
        return View();
    }
}

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EazyMenu.Web.Areas.Restaurant.Controllers;

/// <summary>
/// کنترلر تنظیمات رستوران
/// </summary>
[Area("Restaurant")]
[Authorize(Roles = "RestaurantOwner,Admin")]
public class SettingsController : BaseRestaurantController
{
    public SettingsController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// صفحه تنظیمات رستوران
    /// Route: /Restaurant/Settings/Index
    /// </summary>
    public IActionResult Index()
    {
        // RestaurantId, RestaurantSlug, RestaurantName از BaseRestaurantController تزریق شده‌اند
        return View();
    }

    /// <summary>
    /// صفحه تنظیمات عمومی
    /// </summary>
    public IActionResult General()
    {
        return View();
    }

    /// <summary>
    /// صفحه تنظیمات پرداخت
    /// </summary>
    public IActionResult Payment()
    {
        return View();
    }

    /// <summary>
    /// صفحه تنظیمات اعلان‌ها
    /// </summary>
    public IActionResult Notifications()
    {
        return View();
    }
}

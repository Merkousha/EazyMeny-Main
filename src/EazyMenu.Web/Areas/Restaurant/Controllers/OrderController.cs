using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EazyMenu.Web.Areas.Restaurant.Controllers;

/// <summary>
/// کنترلر مدیریت سفارشات رستوران
/// </summary>
[Area("Restaurant")]
[Authorize(Roles = "RestaurantOwner")]
public class OrderController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Details(Guid id)
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateStatus(Guid id, string status)
    {
        // TODO: Implement order status update
        TempData["Success"] = "وضعیت سفارش با موفقیت به‌روزرسانی شد";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Print(Guid id)
    {
        // TODO: Implement order print
        return View();
    }
}

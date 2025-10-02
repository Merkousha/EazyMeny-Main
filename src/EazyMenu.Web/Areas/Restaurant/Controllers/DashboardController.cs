using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EazyMenu.Web.Areas.Restaurant.Controllers;

/// <summary>
/// داشبورد پنل مدیریت رستوران
/// </summary>
public class DashboardController : BaseRestaurantController
{
    public DashboardController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// صفحه اصلی داشبورد رستوران
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        // نمایش اطلاعات اولیه رستوران
        ViewBag.RestaurantName = ViewData["RestaurantName"];
        ViewBag.RestaurantSlug = ViewData["RestaurantSlug"];
        
        return View();
    }
}

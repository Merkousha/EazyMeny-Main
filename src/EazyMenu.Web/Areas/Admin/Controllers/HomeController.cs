using EazyMenu.Application.Features.Dashboard.Queries.GetDashboardStats;
using EazyMenu.Application.Features.Dashboard.Queries.GetRecentRestaurants;
using EazyMenu.Web.Areas.Admin.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EazyMenu.Web.Areas.Admin.Controllers;

/// <summary>
/// کنترلر صفحه اصلی پنل ادمین (داشبورد)
/// </summary>
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class HomeController : Controller
{
    private readonly IMediator _mediator;

    public HomeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// صفحه اصلی داشبورد ادمین
    /// </summary>
    public async Task<IActionResult> Index()
    {
        // دریافت آمار کلی
        var stats = await _mediator.Send(new GetDashboardStatsQuery());

        // دریافت آخرین رستوران‌ها
        var recentRestaurants = await _mediator.Send(new GetRecentRestaurantsQuery(5));

        var viewModel = new DashboardViewModel
        {
            Stats = stats,
            RecentRestaurants = recentRestaurants
        };

        return View(viewModel);
    }
}

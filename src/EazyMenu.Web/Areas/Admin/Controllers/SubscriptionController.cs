using EazyMenu.Application.Features.Subscriptions.Queries.GetAllSubscriptions;
using EazyMenu.Application.Features.Subscriptions.Queries.GetSubscriptionDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EazyMenu.Web.Areas.Admin.Controllers;

/// <summary>
/// کنترلر مدیریت اشتراک‌ها در پنل ادمین
/// </summary>
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SubscriptionController : Controller
{
    private readonly IMediator _mediator;

    public SubscriptionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// لیست اشتراک‌ها
    /// </summary>
    public async Task<IActionResult> Index(Guid? restaurantId, string? status)
    {
        var subscriptions = await _mediator.Send(new GetAllSubscriptionsQuery
        {
            RestaurantId = restaurantId,
            Status = status
        });
        
        ViewData["RestaurantId"] = restaurantId;
        ViewData["Status"] = status;
        
        return View(subscriptions);
    }

    /// <summary>
    /// جزئیات اشتراک
    /// </summary>
    public async Task<IActionResult> Details(Guid id)
    {
        var subscription = await _mediator.Send(new GetSubscriptionDetailsQuery(id));
        if (subscription is null)
        {
            TempData["Error"] = "اشتراک مورد نظر یافت نشد";
            return RedirectToAction(nameof(Index));
        }

        return View(subscription);
    }
}

using EazyMenu.Application.Features.Orders.Commands.UpdateOrderStatus;
using EazyMenu.Application.Features.Orders.Queries.GetAllOrders;
using EazyMenu.Application.Features.Orders.Queries.GetOrderDetails;
using EazyMenu.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EazyMenu.Web.Areas.Admin.Controllers;

/// <summary>
/// کنترلر مدیریت سفارش‌ها در پنل ادمین
/// </summary>
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class OrderController : Controller
{
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// لیست سفارش‌ها
    /// </summary>
    public async Task<IActionResult> Index(Guid? restaurantId, EazyMenu.Domain.Enums.OrderStatus? status)
    {
        var orders = await _mediator.Send(new GetAllOrdersQuery(restaurantId, status));
        
        ViewData["RestaurantId"] = restaurantId;
        ViewData["Status"] = status;
        return View(orders);
    }

    /// <summary>
    /// جزئیات سفارش
    /// </summary>
    public async Task<IActionResult> Details(Guid id)
    {
        var order = await _mediator.Send(new GetOrderDetailsQuery(id));
        if (order is null)
        {
            TempData["Error"] = "سفارش مورد نظر یافت نشد";
            return RedirectToAction(nameof(Index));
        }

        return View(order);
    }

    /// <summary>
    /// تغییر وضعیت سفارش
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(Guid id, OrderStatus newStatus, string? cancellationReason = null)
    {
        var command = new UpdateOrderStatusCommand(id, newStatus, cancellationReason);
        var result = await _mediator.Send(command);

        if (result)
        {
            TempData["Success"] = "وضعیت سفارش با موفقیت به‌روزرسانی شد";
        }
        else
        {
            TempData["Error"] = "خطا در به‌روزرسانی وضعیت سفارش";
        }

        return RedirectToAction(nameof(Details), new { id });
    }
}

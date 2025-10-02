using EazyMenu.Application.Features.Reservations.Commands.ConfirmReservation;
using EazyMenu.Application.Features.Reservations.Queries.GetReservationById;
using EazyMenu.Application.Features.Reservations.Queries.GetReservationsByRestaurant;
using EazyMenu.Application.Features.Reservations.Queries.GetReservationsByDate;
using EazyMenu.Application.Features.Restaurants.Queries.GetRestaurantBySlug;
using EazyMenu.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EazyMenu.Web.Areas.Restaurant.Controllers;

/// <summary>
/// کنترلر مدیریت رزروها در پنل رستوران
/// </summary>
[Area("Restaurant")]
[Authorize(Roles = "RestaurantOwner,Admin")]
public class ReservationController : BaseRestaurantController
{
    public ReservationController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// لیست رزروهای رستوران با فیلتر
    /// Route: /Restaurant/Reservation/Index
    /// </summary>
    public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate, ReservationStatus? status)
    {
        // RestaurantId از BaseRestaurantController
        var restaurantId = GetRestaurantId();

        var query = new GetReservationsByRestaurantQuery
        {
            RestaurantId = restaurantId,
            FromDate = fromDate,
            ToDate = toDate,
            Status = status
        };

        var reservations = await _mediator.Send(query);

        ViewBag.FromDate = fromDate;
        ViewBag.ToDate = toDate;
        ViewBag.Status = status;

        return View(reservations);
    }

    /// <summary>
    /// جزئیات رزرو
    /// Route: /Restaurant/Reservation/Details/{id}
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var query = new GetReservationByIdQuery { Id = id };
        var reservation = await _mediator.Send(query);

        if (reservation == null)
        {
            TempData["ErrorMessage"] = "رزرو یافت نشد.";
            return RedirectToAction(nameof(Index));
        }

        return View(reservation);
    }

    /// <summary>
    /// تایید رزرو
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(Guid id, int? tableNumber)
    {
        try
        {
            var command = new ConfirmReservationCommand
            {
                ReservationId = id,
                TableNumber = tableNumber
            };

            await _mediator.Send(command);

            TempData["SuccessMessage"] = "رزرو با موفقیت تایید شد.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    /// <summary>
    /// نمایش کلندر روزانه رزروها
    /// Route: /Restaurant/Reservation/Calendar
    /// </summary>
    public async Task<IActionResult> Calendar()
    {
        // RestaurantId از BaseRestaurantController
        var restaurantId = GetRestaurantId();

        // Default to today
        var selectedDate = DateTime.Today;

        var query = new GetReservationsByDateQuery
        {
            RestaurantId = restaurantId,
            Date = selectedDate
        };

        var reservations = await _mediator.Send(query);

        ViewBag.SelectedDate = selectedDate;

        return View(reservations);
    }
    
    /// <summary>
    /// دریافت رزروهای یک تاریخ خاص (AJAX)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetReservationsByDate(DateTime date)
    {
        // RestaurantId از BaseRestaurantController
        var restaurantId = GetRestaurantId();

        var query = new GetReservationsByDateQuery
        {
            RestaurantId = restaurantId,
            Date = date
        };

        var reservations = await _mediator.Send(query);

        return Json(new { success = true, data = reservations });
    }

    /// <summary>
    /// لغو رزرو توسط رستوران
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id, string? cancellationReason)
    {
        try
        {
            var command = new EazyMenu.Application.Features.Reservations.Commands.CancelReservation.CancelReservationCommand
            {
                ReservationId = id,
                CancellationReason = cancellationReason ?? "لغو شده توسط رستوران"
            };

            await _mediator.Send(command);

            TempData["SuccessMessage"] = "رزرو با موفقیت لغو شد.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }
}

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
[Authorize(Roles = "RestaurantOwner")]
[Route("Restaurant/Reservation")]
public class ReservationController : Controller
{
    private readonly IMediator _mediator;

    public ReservationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// لیست رزروهای رستوران با فیلتر
    /// Route: /Restaurant/Reservation/Index/{slug}
    /// </summary>
    [HttpGet("Index/{slug}")]
    public async Task<IActionResult> Index(string slug, DateTime? fromDate, DateTime? toDate, ReservationStatus? status)
    {
        // Get restaurant by slug
        var restaurantQuery = new GetRestaurantBySlugQuery { Slug = slug };
        var restaurant = await _mediator.Send(restaurantQuery);

        if (restaurant == null)
        {
            TempData["ErrorMessage"] = "رستوران یافت نشد.";
            return RedirectToAction("Index", "Home", new { area = "Restaurant" });
        }

        var query = new GetReservationsByRestaurantQuery
        {
            RestaurantId = restaurant.Id,
            FromDate = fromDate,
            ToDate = toDate,
            Status = status
        };

        var reservations = await _mediator.Send(query);

        ViewBag.RestaurantSlug = slug;
        ViewBag.RestaurantId = restaurant.Id;
        ViewBag.RestaurantName = restaurant.Name;
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
    /// Route: /Restaurant/Reservation/Calendar/{slug}
    /// </summary>
    [HttpGet("Calendar/{slug}")]
    public async Task<IActionResult> Calendar(string slug)
    {
        // Get restaurant by slug
        var restaurantQuery = new GetRestaurantBySlugQuery { Slug = slug };
        var restaurant = await _mediator.Send(restaurantQuery);

        if (restaurant == null)
        {
            TempData["ErrorMessage"] = "رستوران یافت نشد.";
            return RedirectToAction("Index", "Home", new { area = "Restaurant" });
        }

        // Default to today
        var selectedDate = DateTime.Today;

        var query = new GetReservationsByDateQuery
        {
            RestaurantId = restaurant.Id,
            Date = selectedDate
        };

        var reservations = await _mediator.Send(query);

        ViewBag.RestaurantSlug = slug;
        ViewBag.RestaurantId = restaurant.Id;
        ViewBag.RestaurantName = restaurant.Name;
        ViewBag.SelectedDate = selectedDate;

        return View(reservations);
    }
    
    /// <summary>
    /// دریافت رزروهای یک تاریخ خاص (AJAX)
    /// </summary>
    [HttpGet("Calendar/{slug}/GetByDate")]
    public async Task<IActionResult> GetReservationsByDate(string slug, DateTime date)
    {
        // Get restaurant by slug
        var restaurantQuery = new GetRestaurantBySlugQuery { Slug = slug };
        var restaurant = await _mediator.Send(restaurantQuery);

        if (restaurant == null)
        {
            return NotFound(new { success = false, message = "رستوران یافت نشد." });
        }

        var query = new GetReservationsByDateQuery
        {
            RestaurantId = restaurant.Id,
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

using EazyMenu.Application.Features.Reservations.Commands.CancelReservation;
using EazyMenu.Application.Features.Reservations.Commands.CreateReservation;
using EazyMenu.Application.Features.Reservations.Queries.GetReservationById;
using EazyMenu.Application.Features.Reservations.Queries.GetReservationsByCustomer;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EazyMenu.Web.Controllers;

/// <summary>
/// کنترلر رزرو میز برای مشتریان (Public)
/// </summary>
public class ReservationController : Controller
{
    private readonly IMediator _mediator;

    public ReservationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// فرم رزرو میز برای یک رستوران خاص
    /// Route: /Reservation/Reserve/{restaurantId}
    /// </summary>
    [HttpGet]
    public IActionResult Reserve(Guid restaurantId)
    {
        var command = new CreateReservationCommand
        {
            RestaurantId = restaurantId
        };

        return View(command);
    }

    /// <summary>
    /// ثبت رزرو جدید
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reserve(CreateReservationCommand command)
    {
        if (!ModelState.IsValid)
        {
            return View(command);
        }

        try
        {
            // اگر کاربر لاگین کرده، CustomerId رو set کن
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    command.CustomerId = userId;
                }
            }

            var reservationId = await _mediator.Send(command);

            TempData["SuccessMessage"] = "رزرو شما با موفقیت ثبت شد. پیامک تایید برای شما ارسال شد.";
            return RedirectToAction(nameof(Details), new { id = reservationId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(command);
        }
    }

    /// <summary>
    /// لیست رزروهای من
    /// Route: /Reservation/MyReservations
    /// </summary>
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> MyReservations()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var query = new GetReservationsByCustomerQuery
        {
            CustomerId = userId
        };

        var reservations = await _mediator.Send(query);

        return View(reservations);
    }

    /// <summary>
    /// جزئیات یک رزرو
    /// Route: /Reservation/Details/{id}
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var query = new GetReservationByIdQuery { Id = id };
        var reservation = await _mediator.Send(query);

        if (reservation == null)
        {
            TempData["ErrorMessage"] = "رزرو یافت نشد.";
            return RedirectToAction(nameof(MyReservations));
        }

        return View(reservation);
    }

    /// <summary>
    /// لغو رزرو توسط مشتری
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id, string? cancellationReason)
    {
        try
        {
            var command = new CancelReservationCommand
            {
                ReservationId = id,
                CancellationReason = cancellationReason
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

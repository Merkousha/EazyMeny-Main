using EazyMenu.Application.Features.Subscriptions.Commands.PurchaseSubscription;
using EazyMenu.Application.Features.Subscriptions.Commands.RenewSubscription;
using EazyMenu.Application.Features.Subscriptions.Commands.VerifyPayment;
using EazyMenu.Application.Features.Subscriptions.Queries.GetSubscriptionPlans;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EazyMenu.Web.Controllers;

/// <summary>
/// Controller عمومی برای مدیریت خرید و تمدید اشتراک
/// </summary>
public class SubscriptionController : Controller
{
    private readonly IMediator _mediator;

    public SubscriptionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// صفحه انتخاب پلن
    /// </summary>
    [Authorize(Roles = "RestaurantOwner")]
    public async Task<IActionResult> ChoosePlan()
    {
        var query = new GetSubscriptionPlansQuery { OnlyActive = true };
        var plans = await _mediator.Send(query);
        return View(plans);
    }

    /// <summary>
    /// خرید اشتراک
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "RestaurantOwner")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Purchase(Guid planId, bool isYearly = false)
    {
        // دریافت RestaurantId از Claim (باید در Register/Login تنظیم شود)
        var restaurantIdClaim = User.FindFirst("RestaurantId")?.Value;
        if (string.IsNullOrEmpty(restaurantIdClaim) || !Guid.TryParse(restaurantIdClaim, out var restaurantId))
        {
            TempData["Error"] = "رستوران یافت نشد. لطفاً دوباره وارد شوید.";
            return RedirectToAction("ChoosePlan");
        }

        var callbackUrl = Url.Action("PaymentCallback", "Subscription", null, Request.Scheme) ?? string.Empty;

        var command = new PurchaseSubscriptionCommand
        {
            SubscriptionPlanId = planId,
            RestaurantId = restaurantId,
            IsYearly = isYearly,
            CallbackUrl = callbackUrl
        };

        var result = await _mediator.Send(command);

        if (!result.Success || string.IsNullOrEmpty(result.PaymentUrl))
        {
            TempData["Error"] = result.ErrorMessage ?? "خطا در ایجاد درخواست پرداخت";
            return RedirectToAction("ChoosePlan");
        }

        // Redirect به درگاه زرین‌پال
        return Redirect(result.PaymentUrl);
    }

    /// <summary>
    /// Callback از زرین‌پال
    /// </summary>
    [Authorize(Roles = "RestaurantOwner")]
    public async Task<IActionResult> PaymentCallback(string Authority, string Status)
    {
        if (string.IsNullOrEmpty(Authority) || string.IsNullOrEmpty(Status))
        {
            return RedirectToAction("Failed", new { message = "اطلاعات پرداخت دریافت نشد" });
        }

        var command = new VerifyPaymentCommand
        {
            Authority = Authority,
            Status = Status
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return RedirectToAction("Failed", new { message = result.ErrorMessage ?? "پرداخت ناموفق" });
        }

        return RedirectToAction("Success", new
        {
            refId = result.RefID,
            amount = result.Amount
        });
    }

    /// <summary>
    /// صفحه موفقیت پرداخت
    /// </summary>
    [Authorize(Roles = "RestaurantOwner")]
    public IActionResult Success(long? refId, decimal amount)
    {
        ViewBag.RefID = refId;
        ViewBag.Amount = amount;
        return View();
    }

    /// <summary>
    /// صفحه خطا در پرداخت
    /// </summary>
    [Authorize(Roles = "RestaurantOwner")]
    public IActionResult Failed(string? message)
    {
        ViewBag.ErrorMessage = message ?? "پرداخت ناموفق بود";
        return View();
    }

    /// <summary>
    /// تمدید اشتراک
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "RestaurantOwner")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Renew(Guid subscriptionId, bool isYearly = false)
    {
        var callbackUrl = Url.Action("PaymentCallback", "Subscription", null, Request.Scheme) ?? string.Empty;

        var command = new RenewSubscriptionCommand
        {
            SubscriptionId = subscriptionId,
            IsYearly = isYearly,
            CallbackUrl = callbackUrl
        };

        var result = await _mediator.Send(command);

        if (!result.Success || string.IsNullOrEmpty(result.PaymentUrl))
        {
            TempData["Error"] = result.ErrorMessage ?? "خطا در ایجاد درخواست پرداخت";
            return RedirectToAction("Index", "Home");
        }

        // Redirect به درگاه زرین‌پال
        return Redirect(result.PaymentUrl);
    }
}

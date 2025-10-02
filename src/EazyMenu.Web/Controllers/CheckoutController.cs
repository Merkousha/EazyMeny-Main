using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Checkout;
using EazyMenu.Application.Features.Orders.Commands.CreateOrder;
using EazyMenu.Application.Features.Orders.Commands.UpdateOrderPayment;
using EazyMenu.Application.Features.Orders.Queries.GetOrderById;
using EazyMenu.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EazyMenu.Web.Controllers;

/// <summary>
/// کنترلر Checkout و تکمیل سفارش
/// </summary>
public class CheckoutController : Controller
{
    private readonly IMediator _mediator;
    private readonly ICartService _cartService;
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IPaymentService _paymentService;

    public CheckoutController(
        IMediator mediator,
        ICartService cartService,
        IRepository<Restaurant> restaurantRepository,
        IPaymentService paymentService)
    {
        _mediator = mediator;
        _cartService = cartService;
        _restaurantRepository = restaurantRepository;
        _paymentService = paymentService;
    }

    /// <summary>
    /// نمایش صفحه Checkout
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // بررسی سبد خرید
        var cart = _cartService.GetCart();
        if (_cartService.IsCartEmpty())
        {
            TempData["Error"] = "سبد خرید شما خالی است.";
            return RedirectToAction("Index", "Home");
        }

        // دریافت اطلاعات رستوران
        var restaurant = await _restaurantRepository.GetByIdAsync(cart.RestaurantId);
        if (restaurant == null)
        {
            TempData["Error"] = "رستوران یافت نشد.";
            return RedirectToAction("Index", "Home");
        }

        // ایجاد CheckoutDto
        var model = new CheckoutDto
        {
            RestaurantId = cart.RestaurantId,
            RestaurantName = cart.RestaurantName,
            TotalAmount = cart.TotalAmount,
            TotalDiscount = cart.TotalDiscount,
            DeliveryFee = restaurant.DeliveryFee,
            IsTakeaway = false
        };

        ViewBag.Cart = cart;
        ViewBag.Restaurant = restaurant;

        return View(model);
    }

    /// <summary>
    /// ثبت سفارش و انتقال به درگاه پرداخت
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(CheckoutDto model)
    {
        try
        {
            // بررسی سبد خرید
            var cart = _cartService.GetCart();
            if (_cartService.IsCartEmpty())
            {
                return Json(new { success = false, message = "سبد خرید خالی است." });
            }

            // Validation
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return Json(new { success = false, message = string.Join(", ", errors) });
            }

            // دریافت اطلاعات رستوران
            var restaurant = await _restaurantRepository.GetByIdAsync(cart.RestaurantId);
            if (restaurant == null)
            {
                return Json(new { success = false, message = "رستوران یافت نشد." });
            }

            // ایجاد Command برای ثبت سفارش
            var command = new CreateOrderCommand
            {
                RestaurantId = cart.RestaurantId,
                CustomerName = model.CustomerName,
                CustomerPhone = model.CustomerPhone,
                DeliveryAddress = model.DeliveryAddress,
                Note = model.Note,
                PreferredDeliveryTime = model.PreferredDeliveryTime,
                IsTakeaway = model.IsTakeaway,
                Items = cart.Items.Select(item => new OrderItemCommand
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price,
                    DiscountedPrice = item.DiscountedPrice,
                    ImageUrl = item.ImageUrl
                }).ToList()
            };

            // ثبت سفارش در دیتابیس
            var orderId = await _mediator.Send(command);

            // محاسبه مبلغ نهایی
            var deliveryFee = model.IsTakeaway ? 0m : restaurant.DeliveryFee;
            var finalAmount = cart.TotalAmount + deliveryFee;

            // درخواست پرداخت از Zarinpal
            var callbackUrl = Url.Action("Verify", "Checkout", null, Request.Scheme);
            var paymentResult = await _paymentService.RequestPaymentAsync(
                amount: finalAmount,
                description: $"پرداخت سفارش {restaurant.Name}",
                callbackUrl: callbackUrl!
            );

            if (!paymentResult.IsSuccess)
            {
                return Json(new 
                { 
                    success = false, 
                    message = $"خطا در ایجاد درخواست پرداخت: {paymentResult.ErrorMessage}" 
                });
            }

            // ذخیره OrderId و Authority در Session
            HttpContext.Session.SetString("OrderId", orderId.ToString());
            HttpContext.Session.SetString("PaymentAuthority", paymentResult.Authority!);

            // بازگشت URL درگاه پرداخت
            return Json(new 
            { 
                success = true, 
                paymentUrl = paymentResult.PaymentUrl 
            });
        }
        catch (Exception ex)
        {
            return Json(new 
            { 
                success = false, 
                message = $"خطا در ثبت سفارش: {ex.Message}" 
            });
        }
    }

    /// <summary>
    /// تایید پرداخت (Callback از Zarinpal)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Verify(string Authority, string Status)
    {
        try
        {
            // بررسی وضعیت پرداخت
            if (Status != "OK")
            {
                ViewBag.Success = false;
                ViewBag.Message = "پرداخت توسط کاربر لغو شد.";
                return View("PaymentResult");
            }

            // دریافت OrderId از Session
            var orderIdStr = HttpContext.Session.GetString("OrderId");
            if (string.IsNullOrEmpty(orderIdStr) || !Guid.TryParse(orderIdStr, out var orderId))
            {
                ViewBag.Success = false;
                ViewBag.Message = "اطلاعات سفارش یافت نشد.";
                return View("PaymentResult");
            }

            // دریافت مبلغ سفارش
            var cart = _cartService.GetCart();
            var restaurant = await _restaurantRepository.GetByIdAsync(cart.RestaurantId);
            var deliveryFee = cart.Items.Any() 
                ? (restaurant?.DeliveryFee ?? 0m) 
                : 0m;
            var finalAmount = cart.TotalAmount + deliveryFee;

            // تایید پرداخت با Zarinpal
            var verifyResult = await _paymentService.VerifyPaymentAsync(
                authority: Authority,
                amount: finalAmount
            );

            if (!verifyResult.IsSuccess)
            {
                ViewBag.Success = false;
                ViewBag.Message = $"پرداخت ناموفق: {verifyResult.ErrorMessage}";
                ViewBag.OrderId = orderId;
                return View("PaymentResult");
            }

            // بروزرسانی وضعیت سفارش
            var order = await _mediator.Send(new GetOrderByIdQuery { OrderId = orderId });
            if (order != null)
            {
                // بروزرسانی وضعیت پرداخت
                await _mediator.Send(new UpdateOrderPaymentCommand
                {
                    OrderId = orderId,
                    IsPaid = true,
                    PaymentAuthority = Authority,
                    PaymentRefId = verifyResult.RefID?.ToString()
                });
            }

            // خالی کردن سبد خرید
            _cartService.ClearCart();

            // پاک کردن Session
            HttpContext.Session.Remove("OrderId");
            HttpContext.Session.Remove("PaymentAuthority");

            // نمایش صفحه موفقیت
            ViewBag.Success = true;
            ViewBag.Message = "پرداخت با موفقیت انجام شد.";
            ViewBag.OrderId = orderId;
            ViewBag.RefId = verifyResult.RefID;
            ViewBag.OrderNumber = order?.OrderNumber;

            return View("PaymentResult");
        }
        catch (Exception ex)
        {
            ViewBag.Success = false;
            ViewBag.Message = $"خطا در تایید پرداخت: {ex.Message}";
            return View("PaymentResult");
        }
    }
}

using EazyMenu.Application.Common.Models.Restaurant;
using EazyMenu.Application.Features.Restaurants.Commands.CreateRestaurant;
using EazyMenu.Application.Features.Restaurants.Commands.DeleteRestaurant;
using EazyMenu.Application.Features.Restaurants.Commands.UpdateRestaurant;
using EazyMenu.Application.Features.Restaurants.Queries.GetAllRestaurants;
using EazyMenu.Application.Features.Restaurants.Queries.GetRestaurantById;
using EazyMenu.Application.Features.Restaurants.Queries.GetRestaurantsByOwner;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EazyMenu.Web.Areas.Admin.Controllers;

/// <summary>
/// کنترلر مدیریت رستوران‌ها در پنل ادمین
/// </summary>
[Area("Admin")]
[Authorize(Roles = "Admin,RestaurantOwner")]
public class RestaurantController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<RestaurantController> _logger;

    public RestaurantController(IMediator mediator, ILogger<RestaurantController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// نمایش لیست رستوران‌ها
    /// Admin: همه رستوران‌ها
    /// RestaurantOwner: فقط رستوران‌های خودش
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            List<RestaurantListDto> restaurants;

            // بررسی نقش کاربر
            if (User.IsInRole("Admin"))
            {
                // ادمین همه رستوران‌ها را می‌بیند
                var query = new GetAllRestaurantsQuery();
                restaurants = await _mediator.Send(query);
            }
            else
            {
                // صاحب رستوران فقط رستوران‌های خودش را می‌بیند
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var query = new GetRestaurantsByOwnerQuery(userId);
                restaurants = await _mediator.Send(query);
            }

            return View(restaurants);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت لیست رستوران‌ها");
            TempData["Error"] = "خطا در دریافت اطلاعات رستوران‌ها";
            return View(new List<RestaurantListDto>());
        }
    }

    /// <summary>
    /// نمایش صفحه ایجاد رستوران جدید
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateRestaurantCommand());
    }

    /// <summary>
    /// ایجاد رستوران جدید
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRestaurantCommand command)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(command);
            }

            // تنظیم OwnerId از کاربر فعلی
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            command.OwnerId = userId;

            // ارسال Command
            var restaurantId = await _mediator.Send(command);

            TempData["Success"] = "رستوران با موفقیت ایجاد شد";
            _logger.LogInformation("رستوران جدید با شناسه {RestaurantId} توسط کاربر {UserId} ایجاد شد", restaurantId, userId);

            return RedirectToAction(nameof(Details), new { id = restaurantId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ایجاد رستوران");
            ModelState.AddModelError(string.Empty, "خطا در ایجاد رستوران. لطفاً دوباره تلاش کنید.");
            return View(command);
        }
    }

    /// <summary>
    /// نمایش جزئیات رستوران
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var query = new GetRestaurantByIdQuery(id);
            var restaurant = await _mediator.Send(query);

            if (restaurant == null)
            {
                TempData["Error"] = "رستوران مورد نظر یافت نشد";
                return RedirectToAction(nameof(Index));
            }

            // بررسی دسترسی: Admin یا صاحب رستوران
            if (!User.IsInRole("Admin"))
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                if (restaurant.OwnerId != userId)
                {
                    TempData["Error"] = "شما به این رستوران دسترسی ندارید";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(restaurant);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت جزئیات رستوران {RestaurantId}", id);
            TempData["Error"] = "خطا در دریافت اطلاعات رستوران";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// نمایش صفحه ویرایش رستوران
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var query = new GetRestaurantByIdQuery(id);
            var restaurant = await _mediator.Send(query);

            if (restaurant == null)
            {
                TempData["Error"] = "رستوران مورد نظر یافت نشد";
                return RedirectToAction(nameof(Index));
            }

            // بررسی دسترسی
            if (!User.IsInRole("Admin"))
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                if (restaurant.OwnerId != userId)
                {
                    TempData["Error"] = "شما به این رستوران دسترسی ندارید";
                    return RedirectToAction(nameof(Index));
                }
            }

            // تبدیل DTO به Command
            var command = new UpdateRestaurantCommand
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                NameEn = restaurant.NameEn,
                Description = restaurant.Description,
                ManagerName = restaurant.ManagerName,
                PhoneNumber = restaurant.PhoneNumber,
                Email = restaurant.Email,
                Address = restaurant.Address,
                WorkingHours = restaurant.WorkingHours,
                IsActive = restaurant.IsActive,
                AcceptOnlineOrders = restaurant.AcceptOnlineOrders,
                AcceptReservations = restaurant.AcceptReservations,
                DeliveryFee = restaurant.DeliveryFee,
                MinimumOrderAmount = restaurant.MinimumOrderAmount
            };

            return View(command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت اطلاعات رستوران برای ویرایش {RestaurantId}", id);
            TempData["Error"] = "خطا در دریافت اطلاعات رستوران";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// ویرایش رستوران
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateRestaurantCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                TempData["Error"] = "اطلاعات نامعتبر است";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(command);
            }

            // بررسی دسترسی
            var query = new GetRestaurantByIdQuery(id);
            var restaurant = await _mediator.Send(query);

            if (restaurant == null)
            {
                TempData["Error"] = "رستوران مورد نظر یافت نشد";
                return RedirectToAction(nameof(Index));
            }

            if (!User.IsInRole("Admin"))
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                if (restaurant.OwnerId != userId)
                {
                    TempData["Error"] = "شما به این رستوران دسترسی ندارید";
                    return RedirectToAction(nameof(Index));
                }
            }

            // ارسال Command
            var result = await _mediator.Send(command);

            if (result)
            {
                TempData["Success"] = "رستوران با موفقیت ویرایش شد";
                _logger.LogInformation("رستوران {RestaurantId} توسط کاربر {UserId} ویرایش شد", id, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction(nameof(Details), new { id });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "خطا در ویرایش رستوران");
                return View(command);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ویرایش رستوران {RestaurantId}", id);
            ModelState.AddModelError(string.Empty, "خطا در ویرایش رستوران. لطفاً دوباره تلاش کنید.");
            return View(command);
        }
    }

    /// <summary>
    /// حذف رستوران (Soft Delete)
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            // بررسی دسترسی
            var query = new GetRestaurantByIdQuery(id);
            var restaurant = await _mediator.Send(query);

            if (restaurant == null)
            {
                TempData["Error"] = "رستوران مورد نظر یافت نشد";
                return RedirectToAction(nameof(Index));
            }

            if (!User.IsInRole("Admin"))
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                if (restaurant.OwnerId != userId)
                {
                    TempData["Error"] = "شما به این رستوران دسترسی ندارید";
                    return RedirectToAction(nameof(Index));
                }
            }

            // ارسال Command
            var command = new DeleteRestaurantCommand(id);
            var result = await _mediator.Send(command);

            if (result)
            {
                TempData["Success"] = "رستوران با موفقیت حذف شد";
                _logger.LogInformation("رستوران {RestaurantId} توسط کاربر {UserId} حذف شد", id, User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            else
            {
                TempData["Error"] = "خطا در حذف رستوران";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در حذف رستوران {RestaurantId}", id);
            TempData["Error"] = "خطا در حذف رستوران. لطفاً دوباره تلاش کنید.";
            return RedirectToAction(nameof(Index));
        }
    }
}

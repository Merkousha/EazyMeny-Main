using EazyMenu.Application.Features.AI.Commands.SaveAiSettings;
using EazyMenu.Application.Features.AI.Commands.TestAiConnection;
using EazyMenu.Application.Features.AI.Queries.GetAiSettings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EazyMenu.Web.Areas.Restaurant.Controllers;

/// <summary>
/// کنترلر تنظیمات هوش مصنوعی
/// </summary>
public class AiSettingsController : BaseRestaurantController
{
    private readonly ILogger<AiSettingsController> _logger;

    public AiSettingsController(IMediator mediator, ILogger<AiSettingsController> logger)
        : base(mediator)
    {
        _logger = logger;
    }

    /// <summary>
    /// صفحه تنظیمات هوش مصنوعی
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            var restaurantId = GetRestaurantId();
            var query = new GetAiSettingsQuery { RestaurantId = restaurantId };
            var settings = await _mediator.Send(query);

            return View(settings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در بارگذاری تنظیمات AI");
            TempData["Error"] = "خطا در بارگذاری تنظیمات";
            return RedirectToAction("Index", "Dashboard");
        }
    }

    /// <summary>
    /// ذخیره تنظیمات
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(SaveAiSettingsCommand command)
    {
        try
        {
            command.RestaurantId = GetRestaurantId();

            var result = await _mediator.Send(command);

            TempData["Success"] = "تنظیمات با موفقیت ذخیره شد";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ذخیره تنظیمات AI");
            TempData["Error"] = "خطا در ذخیره تنظیمات";
        }

        return Redirect("/Restaurant/AiSettings/Index");
    }

    /// <summary>
    /// تست اتصال
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> TestConnection()
    {
        try
        {
            var restaurantId = GetRestaurantId();
            var command = new TestAiConnectionCommand { RestaurantId = restaurantId };
            var result = await _mediator.Send(command);

            return Json(new
            {
                success = result.IsSuccess,
                message = result.Message,
                responseTime = result.ResponseTimeMs
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در تست اتصال AI");
            return Json(new
            {
                success = false,
                message = $"خطا در تست اتصال: {ex.Message}"
            });
        }
    }
}

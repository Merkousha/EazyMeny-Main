using EazyMenu.Application.Features.Website.Queries.GetRestaurantWebsite;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EazyMenu.Web.Controllers;

/// <summary>
/// کنترلر عمومی نمایش وب‌سایت رستوران‌ها
/// </summary>
public class WebsiteController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<WebsiteController> _logger;

    public WebsiteController(IMediator mediator, ILogger<WebsiteController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// نمایش وب‌سایت رستوران با استفاده از Slug
    /// مسیر: /Website/{slug}
    /// </summary>
    [HttpGet("/Website/{slug}")]
    public async Task<IActionResult> Index(string slug)
    {
        try
        {
            // دریافت اطلاعات وب‌سایت فقط اگر منتشر شده باشد
            var query = new GetRestaurantWebsiteQuery
            {
                RestaurantSlug = slug,
                OnlyPublished = true
            };

            var website = await _mediator.Send(query);

            // اگر وب‌سایت یافت نشد یا منتشر نشده
            if (website == null || !website.IsPublished)
            {
                _logger.LogWarning("Website not found or not published for slug: {Slug}", slug);
                return View("NotPublished");
            }

            // لاگ بازدید
            _logger.LogInformation("Website viewed: {RestaurantName} ({Slug})", website.RestaurantName, slug);

            return View(website);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error displaying website for slug: {Slug}", slug);
            return View("Error");
        }
    }

    /// <summary>
    /// پیش‌نمایش وب‌سایت (برای صاحبان رستوران - بدون نیاز به انتشار)
    /// مسیر: /Website/Preview/{slug}
    /// </summary>
    [HttpGet("/Website/Preview/{slug}")]
    public async Task<IActionResult> Preview(string slug)
    {
        try
        {
            // دریافت اطلاعات وب‌سایت حتی اگر منتشر نشده
            var query = new GetRestaurantWebsiteQuery
            {
                RestaurantSlug = slug,
                OnlyPublished = false // نمایش حتی اگر منتشر نشده
            };

            var website = await _mediator.Send(query);

            if (website == null)
            {
                _logger.LogWarning("Website not found for preview: {Slug}", slug);
                return NotFound();
            }

            // نمایش پیام preview
            ViewBag.IsPreview = true;
            ViewBag.PreviewMessage = "حالت پیش‌نمایش - این وب‌سایت هنوز منتشر نشده است";

            return View("Index", website);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error previewing website for slug: {Slug}", slug);
            return View("Error");
        }
    }
}

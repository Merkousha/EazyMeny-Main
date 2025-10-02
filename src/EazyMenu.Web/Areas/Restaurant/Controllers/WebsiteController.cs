using EazyMenu.Application.Features.Restaurants.Queries.GetRestaurantBySlug;
using EazyMenu.Application.Features.Website.Commands.PublishWebsite;
using EazyMenu.Application.Features.Website.Commands.SelectTemplate;
using EazyMenu.Application.Features.Website.Commands.UpdateContent;
using EazyMenu.Application.Features.Website.Commands.UpdateCustomization;
using EazyMenu.Application.Features.Website.Queries.GetAllTemplates;
using EazyMenu.Application.Features.Website.Queries.GetRestaurantWebsite;
using EazyMenu.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EazyMenu.Web.Areas.Restaurant.Controllers;

/// <summary>
/// کنترلر مدیریت وب‌سایت رستوران
/// </summary>
[Area("Restaurant")]
[Authorize(Roles = "RestaurantOwner,Admin")]
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
    /// داشبورد وب‌سایت - نمای کلی
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(string slug)
    {
        try
        {
            // دریافت RestaurantId از Slug
            var restaurantId = await GetRestaurantIdFromSlugAsync(slug);
            if (restaurantId == Guid.Empty)
            {
                TempData["Error"] = "رستوران یافت نشد";
                return RedirectToAction("Index", "Dashboard", new { area = "Restaurant" });
            }

            // دریافت اطلاعات وب‌سایت
            var query = new GetRestaurantWebsiteQuery
            {
                RestaurantId = restaurantId,
                OnlyPublished = false // نمایش حتی اگر منتشر نشده
            };

            var website = await _mediator.Send(query);

            ViewBag.RestaurantSlug = slug;
            ViewBag.HasWebsite = website != null;
            ViewBag.IsPublished = website?.IsPublished ?? false;

            return View(website);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading website dashboard for slug: {Slug}", slug);
            TempData["Error"] = "خطا در بارگذاری داشبورد وب‌سایت";
            return RedirectToAction("Index", "Dashboard", new { area = "Restaurant" });
        }
    }

    /// <summary>
    /// گالری قالب‌ها
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Templates(string slug)
    {
        try
        {
            var restaurantId = await GetRestaurantIdFromSlugAsync(slug);
            if (restaurantId == Guid.Empty)
            {
                TempData["Error"] = "رستوران یافت نشد";
                return RedirectToAction("Index", new { slug });
            }

            // دریافت تمام قالب‌های فعال
            var query = new GetAllTemplatesQuery
            {
                OnlyActive = true,
                OnlyFree = false
            };

            var templates = await _mediator.Send(query);

            ViewBag.RestaurantSlug = slug;
            ViewBag.RestaurantId = restaurantId;

            return View(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading templates for slug: {Slug}", slug);
            TempData["Error"] = "خطا در بارگذاری قالب‌ها";
            return RedirectToAction("Index", new { slug });
        }
    }

    /// <summary>
    /// انتخاب قالب
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SelectTemplate(string slug, Guid templateId)
    {
        try
        {
            var restaurantId = await GetRestaurantIdFromSlugAsync(slug);
            if (restaurantId == Guid.Empty)
            {
                TempData["Error"] = "رستوران یافت نشد";
                return RedirectToAction("Index", new { slug });
            }

            var command = new SelectTemplateCommand
            {
                RestaurantId = restaurantId,
                TemplateId = templateId
            };

            await _mediator.Send(command);

            TempData["Success"] = "قالب با موفقیت انتخاب شد! حالا می‌توانید آن را سفارشی‌سازی کنید";
            return RedirectToAction("Customize", new { slug });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting template for slug: {Slug}, TemplateId: {TemplateId}", slug, templateId);
            TempData["Error"] = $"خطا در انتخاب قالب: {ex.Message}";
            return RedirectToAction("Templates", new { slug });
        }
    }

    /// <summary>
    /// صفحه سفارشی‌سازی (رنگ، فونت، لوگو، SEO)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Customize(string slug)
    {
        try
        {
            var restaurantId = await GetRestaurantIdFromSlugAsync(slug);
            if (restaurantId == Guid.Empty)
            {
                TempData["Error"] = "رستوران یافت نشد";
                return RedirectToAction("Index", new { slug });
            }

            // دریافت تنظیمات فعلی
            var query = new GetRestaurantWebsiteQuery
            {
                RestaurantId = restaurantId,
                OnlyPublished = false
            };

            var website = await _mediator.Send(query);

            if (website == null || website.Template == null)
            {
                TempData["Warning"] = "ابتدا یک قالب انتخاب کنید";
                return RedirectToAction("Templates", new { slug });
            }

            ViewBag.RestaurantSlug = slug;
            ViewBag.RestaurantId = restaurantId;

            return View(website.Customization);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading customize page for slug: {Slug}", slug);
            TempData["Error"] = "خطا در بارگذاری صفحه سفارشی‌سازی";
            return RedirectToAction("Index", new { slug });
        }
    }

    /// <summary>
    /// ذخیره تنظیمات سفارشی‌سازی
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Customize(string slug, UpdateCustomizationCommand command)
    {
        try
        {
            var restaurantId = await GetRestaurantIdFromSlugAsync(slug);
            if (restaurantId == Guid.Empty)
            {
                TempData["Error"] = "رستوران یافت نشد";
                return RedirectToAction("Index", new { slug });
            }

            command.RestaurantId = restaurantId;

            if (!ModelState.IsValid)
            {
                return View(command);
            }

            await _mediator.Send(command);

            TempData["Success"] = "تنظیمات سفارشی‌سازی با موفقیت ذخیره شد";
            return RedirectToAction("Index", new { slug });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving customization for slug: {Slug}", slug);
            TempData["Error"] = $"خطا در ذخیره تنظیمات: {ex.Message}";
            return View(command);
        }
    }

    /// <summary>
    /// صفحه ویرایش محتوای یک بخش
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> EditContent(string slug, SectionType sectionType)
    {
        try
        {
            var restaurantId = await GetRestaurantIdFromSlugAsync(slug);
            if (restaurantId == Guid.Empty)
            {
                TempData["Error"] = "رستوران یافت نشد";
                return RedirectToAction("Index", new { slug });
            }

            // دریافت محتوای فعلی
            var query = new GetRestaurantWebsiteQuery
            {
                RestaurantId = restaurantId,
                OnlyPublished = false
            };

            var website = await _mediator.Send(query);

            if (website == null || website.Template == null)
            {
                TempData["Warning"] = "ابتدا یک قالب انتخاب کنید";
                return RedirectToAction("Templates", new { slug });
            }

            var content = website.Contents.FirstOrDefault(c => c.SectionType == sectionType);
            var section = website.Template.Sections.FirstOrDefault(s => s.SectionType == sectionType);

            ViewBag.RestaurantSlug = slug;
            ViewBag.RestaurantId = restaurantId;
            ViewBag.SectionType = sectionType;
            ViewBag.SectionTitle = section?.Title ?? sectionType.ToString();
            ViewBag.IsEditable = section?.IsEditable ?? true;

            return View(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading edit content page for slug: {Slug}, Section: {Section}", slug, sectionType);
            TempData["Error"] = "خطا در بارگذاری صفحه ویرایش";
            return RedirectToAction("Index", new { slug });
        }
    }

    /// <summary>
    /// ذخیره محتوای ویرایش شده
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditContent(string slug, UpdateContentCommand command)
    {
        try
        {
            var restaurantId = await GetRestaurantIdFromSlugAsync(slug);
            if (restaurantId == Guid.Empty)
            {
                TempData["Error"] = "رستوران یافت نشد";
                return RedirectToAction("Index", new { slug });
            }

            command.RestaurantId = restaurantId;

            if (!ModelState.IsValid)
            {
                ViewBag.RestaurantSlug = slug;
                ViewBag.SectionType = command.SectionType;
                return View(command);
            }

            await _mediator.Send(command);

            TempData["Success"] = "محتوا با موفقیت ذخیره شد";
            return RedirectToAction("Index", new { slug });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving content for slug: {Slug}, Section: {Section}", slug, command.SectionType);
            TempData["Error"] = $"خطا در ذخیره محتوا: {ex.Message}";
            ViewBag.RestaurantSlug = slug;
            ViewBag.SectionType = command.SectionType;
            return View(command);
        }
    }

    /// <summary>
    /// انتشار وب‌سایت
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publish(string slug)
    {
        try
        {
            var restaurantId = await GetRestaurantIdFromSlugAsync(slug);
            if (restaurantId == Guid.Empty)
            {
                TempData["Error"] = "رستوران یافت نشد";
                return RedirectToAction("Index", new { slug });
            }

            var command = new PublishWebsiteCommand
            {
                RestaurantId = restaurantId,
                IsPublished = true
            };

            await _mediator.Send(command);

            TempData["Success"] = "🎉 وب‌سایت شما با موفقیت منتشر شد!";
            return RedirectToAction("Index", new { slug });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing website for slug: {Slug}", slug);
            TempData["Error"] = $"خطا در انتشار وب‌سایت: {ex.Message}";
            return RedirectToAction("Index", new { slug });
        }
    }

    /// <summary>
    /// لغو انتشار وب‌سایت
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unpublish(string slug)
    {
        try
        {
            var restaurantId = await GetRestaurantIdFromSlugAsync(slug);
            if (restaurantId == Guid.Empty)
            {
                TempData["Error"] = "رستوران یافت نشد";
                return RedirectToAction("Index", new { slug });
            }

            var command = new PublishWebsiteCommand
            {
                RestaurantId = restaurantId,
                IsPublished = false
            };

            await _mediator.Send(command);

            TempData["Success"] = "وب‌سایت از حالت انتشار خارج شد";
            return RedirectToAction("Index", new { slug });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unpublishing website for slug: {Slug}", slug);
            TempData["Error"] = $"خطا در لغو انتشار: {ex.Message}";
            return RedirectToAction("Index", new { slug });
        }
    }

    /// <summary>
    /// پیش‌نمایش وب‌سایت
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Preview(string slug)
    {
        try
        {
            var restaurantId = await GetRestaurantIdFromSlugAsync(slug);
            if (restaurantId == Guid.Empty)
            {
                TempData["Error"] = "رستوران یافت نشد";
                return RedirectToAction("Index", new { slug });
            }

            // Redirect to public website controller (will create in next task)
            return RedirectToAction("Index", "Website", new { area = "", slug });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error previewing website for slug: {Slug}", slug);
            TempData["Error"] = "خطا در نمایش پیش‌نمایش";
            return RedirectToAction("Index", new { slug });
        }
    }

    #region Helper Methods

    /// <summary>
    /// دریافت RestaurantId از Slug
    /// </summary>
    private async Task<Guid> GetRestaurantIdFromSlugAsync(string slug)
    {
        try
        {
            var query = new GetRestaurantBySlugQuery { Slug = slug };
            var restaurant = await _mediator.Send(query);
            return restaurant?.Id ?? Guid.Empty;
        }
        catch
        {
            return Guid.Empty;
        }
    }

    #endregion
}

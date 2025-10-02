using EazyMenu.Application.Features.Website.Commands.PublishWebsite;
using EazyMenu.Application.Features.Website.Commands.SelectTemplate;
using EazyMenu.Application.Features.Website.Commands.UpdateContent;
using EazyMenu.Application.Features.Website.Commands.UpdateCustomization;
using EazyMenu.Application.Features.Website.Queries.GetAllTemplates;
using EazyMenu.Application.Features.Website.Queries.GetRestaurantWebsite;
using EazyMenu.Domain.Enums;
using EazyMenu.Web.Areas.Restaurant.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EazyMenu.Web.Areas.Restaurant.Controllers;

/// <summary>
/// کنترلر مدیریت وب‌سایت رستوران - امن با بررسی Owner
/// </summary>
public class WebsiteController : BaseRestaurantController
{
    private readonly ILogger<WebsiteController> _logger;

    public WebsiteController(IMediator mediator, ILogger<WebsiteController> logger) 
        : base(mediator)
    {
        _logger = logger;
    }

    /// <summary>
    /// داشبورد وب‌سایت - نمای کلی
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var restaurantId = GetRestaurantId();
            
            if (restaurantId == Guid.Empty)
            {
                TempData["Error"] = "رستورانی برای شما یافت نشد";
                return RedirectToAction("Index", "Dashboard", new { area = "Restaurant" });
            }

            var query = new GetRestaurantWebsiteQuery
            {
                RestaurantId = restaurantId,
                OnlyPublished = false
            };

            var website = await _mediator.Send(query);

            ViewBag.HasWebsite = website != null;
            ViewBag.IsPublished = website?.IsPublished ?? false;

            return View(website);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading website dashboard");
            TempData["Error"] = "خطا در بارگذاری داشبورد وب‌سایت";
            return RedirectToAction("Index", "Dashboard", new { area = "Restaurant" });
        }
    }

    /// <summary>
    /// گالری قالب‌ها
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Templates()
    {
        try
        {
            var restaurantId = GetRestaurantId();
            
            if (restaurantId == Guid.Empty)
            {
                TempData["Error"] = "رستورانی برای شما یافت نشد";
                return Redirect("/Restaurant/Website/Index");
            }

            var query = new GetAllTemplatesQuery
            {
                OnlyActive = true,
                OnlyFree = false
            };

            var templates = await _mediator.Send(query);

            return View(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading templates");
            TempData["Error"] = "خطا در بارگذاری قالب‌ها";
            return Redirect("/Restaurant/Website/Index");
        }
    }

    /// <summary>
    /// انتخاب قالب
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SelectTemplate(Guid templateId)
    {
        try
        {
            var restaurantId = GetRestaurantId();
            
            if (restaurantId == Guid.Empty)
            {
                TempData["Error"] = "رستورانی برای شما یافت نشد";
                return Redirect("/Restaurant/Website/Index");
            }

            var command = new SelectTemplateCommand
            {
                RestaurantId = restaurantId,
                TemplateId = templateId
            };

            await _mediator.Send(command);

            TempData["Success"] = "قالب با موفقیت انتخاب شد!";
            return Redirect("/Restaurant/Website/Customize");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting template: {TemplateId}", templateId);
            TempData["Error"] = $"خطا در انتخاب قالب: {ex.Message}";
            return Redirect("/Restaurant/Website/Templates");
        }
    }

    /// <summary>
    /// صفحه سفارشی‌سازی
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Customize()
    {
        try
        {
            var restaurantId = GetRestaurantId();
            
            if (restaurantId == Guid.Empty)
            {
                TempData["Error"] = "رستورانی برای شما یافت نشد";
                return RedirectToAction("Index");
            }

            var query = new GetRestaurantWebsiteQuery
            {
                RestaurantId = restaurantId,
                OnlyPublished = false
            };

            var website = await _mediator.Send(query);

            if (website == null || website.Template == null)
            {
                TempData["Warning"] = "ابتدا یک قالب انتخاب کنید";
                return RedirectToAction("Templates");
            }

            return View(website.Customization);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading customize page");
            TempData["Error"] = "خطا در بارگذاری صفحه سفارشی‌سازی";
            return Redirect("/Restaurant/Website/Index");
        }
    }

    /// <summary>
    /// ذخیره تنظیمات سفارشی‌سازی
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Customize(UpdateCustomizationCommand command)
    {
        try
        {
            var restaurantId = GetRestaurantId();
            
            if (restaurantId == Guid.Empty)
            {
                TempData["Error"] = "رستورانی برای شما یافت نشد";
                return Redirect("/Restaurant/Website/Index");
            }

            command.RestaurantId = restaurantId;

            if (!ModelState.IsValid)
            {
                return View(command);
            }

            await _mediator.Send(command);

            TempData["Success"] = "تنظیمات سفارشی‌سازی با موفقیت ذخیره شد";
            return Redirect("/Restaurant/Website/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving customization");
            TempData["Error"] = $"خطا در ذخیره تنظیمات: {ex.Message}";
            return View(command);
        }
    }

    /// <summary>
    /// صفحه ویرایش محتوای یک بخش
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> EditContent(SectionType sectionType)
    {
        try
        {
            var restaurantId = GetRestaurantId();
            
            if (restaurantId == Guid.Empty)
            {
                TempData["Error"] = "رستورانی برای شما یافت نشد";
                return Redirect("/Restaurant/Website/Index");
            }

            var query = new GetRestaurantWebsiteQuery
            {
                RestaurantId = restaurantId,
                OnlyPublished = false
            };

            var website = await _mediator.Send(query);

            if (website == null || website.Template == null)
            {
                TempData["Warning"] = "ابتدا یک قالب انتخاب کنید";
                return Redirect("/Restaurant/Website/Templates");
            }

            var content = website.Contents.FirstOrDefault(c => c.SectionType == sectionType);
            var section = website.Template.Sections.FirstOrDefault(s => s.SectionType == sectionType);

            var viewModel = new EditContentViewModel
            {
                RestaurantId = restaurantId,
                ContentId = content?.Id,
                SectionType = sectionType,
                SectionTitle = section?.Title ?? sectionType.ToString(),
                IsRequired = section?.IsRequired ?? false,
                IsEditable = section?.IsEditable ?? true,
                CustomContent = content?.Content,
                DefaultContent = null, // Template sections don't store default content in DTO
                UseDefaultContent = content?.UseDefaultContent ?? true,
                IsVisible = content?.IsVisible ?? true
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading edit content page for section: {Section}", sectionType);
            TempData["Error"] = "خطا در بارگذاری صفحه ویرایش";
            return Redirect("/Restaurant/Website/Index");
        }
    }

    /// <summary>
    /// ذخیره محتوای ویرایش شده
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditContent(EditContentViewModel viewModel)
    {
        try
        {
            var restaurantId = GetRestaurantId();
            
            if (restaurantId == Guid.Empty)
            {
                TempData["Error"] = "رستورانی برای شما یافت نشد";
                return Redirect("/Restaurant/Website/Index");
            }

            if (!ModelState.IsValid)
            {
                // بازیابی اطلاعات بخش برای نمایش دوباره فرم
                var websiteQuery = new GetRestaurantWebsiteQuery
                {
                    RestaurantId = restaurantId,
                    OnlyPublished = false
                };

                var website = await _mediator.Send(websiteQuery);
                
                if (website != null && website.Template != null)
                {
                    var section = website.Template.Sections.FirstOrDefault(s => s.SectionType == viewModel.SectionType);
                    viewModel.SectionTitle = section?.Title ?? viewModel.SectionType.ToString();
                    viewModel.IsRequired = section?.IsRequired ?? false;
                    viewModel.IsEditable = section?.IsEditable ?? true;
                }
                
                return View(viewModel);
            }

            // ایجاد Command از ViewModel
            var command = new UpdateContentCommand
            {
                RestaurantId = restaurantId,
                SectionType = viewModel.SectionType,
                Content = viewModel.CustomContent ?? string.Empty,
                UseDefaultContent = viewModel.UseDefaultContent,
                IsVisible = viewModel.IsVisible
            };

            await _mediator.Send(command);

            TempData["Success"] = "محتوا با موفقیت ذخیره شد";
            return Redirect("/Restaurant/Website/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving content for section: {Section}", viewModel.SectionType);
            TempData["Error"] = $"خطا در ذخیره محتوا: {ex.Message}";
            
            // بازیابی اطلاعات بخش برای نمایش خطا
            try
            {
                var restaurantId = GetRestaurantId();
                var websiteQuery = new GetRestaurantWebsiteQuery
                {
                    RestaurantId = restaurantId,
                    OnlyPublished = false
                };

                var website = await _mediator.Send(websiteQuery);
                
                if (website != null && website.Template != null)
                {
                    var section = website.Template.Sections.FirstOrDefault(s => s.SectionType == viewModel.SectionType);
                    viewModel.SectionTitle = section?.Title ?? viewModel.SectionType.ToString();
                    viewModel.IsRequired = section?.IsRequired ?? false;
                    viewModel.IsEditable = section?.IsEditable ?? true;
                }
            }
            catch
            {
                // در صورت خطا در بازیابی، مقادیر پیش‌فرض
            }
            
            return View(viewModel);
        }
    }

    /// <summary>
    /// انتشار وب‌سایت
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publish()
    {
        try
        {
            var restaurantId = GetRestaurantId();
            
            if (restaurantId == Guid.Empty)
            {
                TempData["Error"] = "رستورانی برای شما یافت نشد";
                return Redirect("/Restaurant/Website/Index");
            }

            var command = new PublishWebsiteCommand
            {
                RestaurantId = restaurantId,
                IsPublished = true
            };

            await _mediator.Send(command);

            TempData["Success"] = "🎉 وب‌سایت شما با موفقیت منتشر شد!";
            return Redirect("/Restaurant/Website/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing website");
            TempData["Error"] = $"خطا در انتشار وب‌سایت: {ex.Message}";
            return Redirect("/Restaurant/Website/Index");
        }
    }

    /// <summary>
    /// لغو انتشار وب‌سایت
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unpublish()
    {
        try
        {
            var restaurantId = GetRestaurantId();
            
            if (restaurantId == Guid.Empty)
            {
                TempData["Error"] = "رستورانی برای شما یافت نشد";
                return Redirect("/Restaurant/Website/Index");
            }

            var command = new PublishWebsiteCommand
            {
                RestaurantId = restaurantId,
                IsPublished = false
            };

            await _mediator.Send(command);

            TempData["Success"] = "وب‌سایت از حالت انتشار خارج شد";
            return Redirect("/Restaurant/Website/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unpublishing website");
            TempData["Error"] = $"خطا در لغو انتشار: {ex.Message}";
            return Redirect("/Restaurant/Website/Index");
        }
    }

    /// <summary>
    /// پیش‌نمایش وب‌سایت
    /// </summary>
    [HttpGet]
    public IActionResult Preview()
    {
        try
        {
            var slug = ViewData["RestaurantSlug"]?.ToString();
            
            if (string.IsNullOrEmpty(slug))
            {
                TempData["Error"] = "رستورانی برای شما یافت نشد";
                return Redirect("/Restaurant/Website/Index");
            }

            // Redirect to public website controller
            return RedirectToAction("Index", "Website", new { area = "", slug });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error previewing website");
            TempData["Error"] = "خطا در نمایش پیش‌نمایش";
            return Redirect("/Restaurant/Website/Index");
        }
    }
}

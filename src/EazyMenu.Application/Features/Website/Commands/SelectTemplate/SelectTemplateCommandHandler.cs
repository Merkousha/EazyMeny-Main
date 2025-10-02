using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using EazyMenu.Domain.Enums;
using MediatR;

namespace EazyMenu.Application.Features.Website.Commands.SelectTemplate;

/// <summary>
/// Handler انتخاب قالب
/// </summary>
public class SelectTemplateCommandHandler : IRequestHandler<SelectTemplateCommand, bool>
{
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IRepository<WebsiteTemplate> _templateRepository;
    private readonly IRepository<TemplateSection> _sectionRepository;
    private readonly IRepository<WebsiteContent> _contentRepository;
    private readonly IRepository<WebsiteCustomization> _customizationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SelectTemplateCommandHandler(
        IRepository<Restaurant> restaurantRepository,
        IRepository<WebsiteTemplate> templateRepository,
        IRepository<TemplateSection> sectionRepository,
        IRepository<WebsiteContent> contentRepository,
        IRepository<WebsiteCustomization> customizationRepository,
        IUnitOfWork unitOfWork)
    {
        _restaurantRepository = restaurantRepository;
        _templateRepository = templateRepository;
        _sectionRepository = sectionRepository;
        _contentRepository = contentRepository;
        _customizationRepository = customizationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SelectTemplateCommand request, CancellationToken cancellationToken)
    {
        // بررسی وجود رستوران
        var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId, cancellationToken);
        if (restaurant == null)
            throw new Exception("رستوران یافت نشد");

        // بررسی وجود قالب
        var template = await _templateRepository.GetByIdAsync(request.TemplateId, cancellationToken);
        if (template == null || !template.IsActive)
            throw new Exception("قالب یافت نشد یا غیرفعال است");

        // دریافت بخش‌های قالب
        var sections = await _sectionRepository.GetAllAsync(cancellationToken);
        var templateSections = sections.Where(s => s.TemplateId == request.TemplateId).ToList();

        // حذف کامل محتوای قبلی (به جای soft delete از Remove استفاده می‌کنیم)
        var existingContents = await _contentRepository.FindAsync(
            c => c.RestaurantId == request.RestaurantId,
            cancellationToken);
        
        if (existingContents.Any())
        {
            foreach (var content in existingContents)
            {
                await _contentRepository.DeleteAsync(content, cancellationToken);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken); // ذخیره حذف قبل از افزودن جدید
        }

        // به‌روزرسانی قالب رستوران
        restaurant.WebsiteTheme = template.Name;
        await _restaurantRepository.UpdateAsync(restaurant, cancellationToken);

        // ایجاد محتوای جدید بر اساس بخش‌های قالب
        foreach (var section in templateSections)
        {
            var content = new WebsiteContent
            {
                RestaurantId = request.RestaurantId,
                TemplateId = request.TemplateId,
                SectionType = section.SectionType,
                Content = section.DefaultContent,
                UseDefaultContent = true,
                DisplayOrder = section.DisplayOrder,
                IsVisible = section.IsRequired
            };

            await _contentRepository.AddAsync(content, cancellationToken);
        }

        // ایجاد یا به‌روزرسانی تنظیمات سفارشی‌سازی
        var customization = await _customizationRepository.GetAllAsync(cancellationToken);
        var existingCustomization = customization.FirstOrDefault(c => c.RestaurantId == request.RestaurantId);

        if (existingCustomization == null)
        {
            // رنگ‌های پیش‌فرض از قالب
            var defaultColors = System.Text.Json.JsonDocument.Parse(template.DefaultColors);
            var defaultFonts = System.Text.Json.JsonDocument.Parse(template.DefaultFonts);

            var newCustomization = new WebsiteCustomization
            {
                RestaurantId = request.RestaurantId,
                PrimaryColor = defaultColors.RootElement.GetProperty("primary").GetString() ?? "#FF6B35",
                SecondaryColor = defaultColors.RootElement.GetProperty("secondary").GetString() ?? "#004E89",
                TextColor = defaultColors.RootElement.GetProperty("text").GetString() ?? "#333333",
                BackgroundColor = defaultColors.RootElement.GetProperty("background").GetString() ?? "#FFFFFF",
                FontFamily = defaultFonts.RootElement.GetProperty("primary").GetString() ?? "IRANSans",
                FontSize = 16,
                SeoTitle = restaurant.Name,
                SeoDescription = restaurant.Description,
                SeoKeywords = $"{restaurant.Name}, رستوران, {restaurant.Address}"
            };

            await _customizationRepository.AddAsync(newCustomization, cancellationToken);
        }

        // ذخیره تغییرات
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

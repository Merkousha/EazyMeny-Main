using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Features.Website.DTOs;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Website.Queries.GetRestaurantWebsite;

/// <summary>
/// Handler دریافت وب‌سایت رستوران
/// </summary>
public class GetRestaurantWebsiteQueryHandler : IRequestHandler<GetRestaurantWebsiteQuery, RestaurantWebsiteDto?>
{
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IRepository<WebsiteTemplate> _templateRepository;
    private readonly IRepository<TemplateSection> _sectionRepository;
    private readonly IRepository<WebsiteContent> _contentRepository;
    private readonly IRepository<WebsiteCustomization> _customizationRepository;

    public GetRestaurantWebsiteQueryHandler(
        IRepository<Restaurant> restaurantRepository,
        IRepository<WebsiteTemplate> templateRepository,
        IRepository<TemplateSection> sectionRepository,
        IRepository<WebsiteContent> contentRepository,
        IRepository<WebsiteCustomization> customizationRepository)
    {
        _restaurantRepository = restaurantRepository;
        _templateRepository = templateRepository;
        _sectionRepository = sectionRepository;
        _contentRepository = contentRepository;
        _customizationRepository = customizationRepository;
    }

    public async Task<RestaurantWebsiteDto?> Handle(GetRestaurantWebsiteQuery request, CancellationToken cancellationToken)
    {
        // پیدا کردن رستوران
        Restaurant? restaurant = null;
        var allRestaurants = await _restaurantRepository.GetAllAsync(cancellationToken);

        if (request.RestaurantId != Guid.Empty)
        {
            restaurant = allRestaurants.FirstOrDefault(r => r.Id == request.RestaurantId);
        }
        else if (!string.IsNullOrEmpty(request.RestaurantSlug))
        {
            restaurant = allRestaurants.FirstOrDefault(r => r.Slug == request.RestaurantSlug);
        }

        if (restaurant == null)
            return null;

        // بررسی وضعیت انتشار
        if (request.OnlyPublished && !restaurant.IsWebsitePublished)
            return null;

        // دریافت محتوا
        var allContents = await _contentRepository.GetAllAsync(cancellationToken);
        var contents = allContents.Where(c => c.RestaurantId == restaurant.Id).ToList();

        if (!contents.Any())
            return null;

        // دریافت قالب
        var templateId = contents.First().TemplateId;
        var template = await _templateRepository.GetByIdAsync(templateId, cancellationToken);
        
        if (template == null)
            return null;

        // دریافت بخش‌های قالب
        var allSections = await _sectionRepository.GetAllAsync(cancellationToken);
        var templateSections = allSections.Where(s => s.TemplateId == templateId).ToList();

        // دریافت تنظیمات سفارشی‌سازی
        var allCustomizations = await _customizationRepository.GetAllAsync(cancellationToken);
        var customization = allCustomizations.FirstOrDefault(c => c.RestaurantId == restaurant.Id);

        // ساخت DTO
        var result = new RestaurantWebsiteDto
        {
            RestaurantId = restaurant.Id,
            RestaurantName = restaurant.Name,
            RestaurantSlug = restaurant.Slug,
            IsPublished = restaurant.IsWebsitePublished,
            PublishedAt = restaurant.WebsitePublishedAt,
            Template = new WebsiteTemplateDto
            {
                Id = template.Id,
                Name = template.Name,
                NameEn = template.NameEn,
                Description = template.Description,
                Type = template.Type,
                TemplatePath = template.TemplatePath,
                ThumbnailUrl = template.ThumbnailUrl,
                PreviewImageUrl = template.PreviewImageUrl,
                IsFree = template.IsFree,
                DisplayOrder = template.DisplayOrder,
                Sections = templateSections
                    .OrderBy(s => s.DisplayOrder)
                    .Select(s => new TemplateSectionDto
                    {
                        Id = s.Id,
                        SectionType = s.SectionType,
                        Title = s.Title,
                        TitleEn = s.TitleEn,
                        IsRequired = s.IsRequired,
                        IsEditable = s.IsEditable,
                        DisplayOrder = s.DisplayOrder
                    })
                    .ToList()
            },
            Customization = customization == null ? null : new WebsiteCustomizationDto
            {
                Id = customization.Id,
                RestaurantId = customization.RestaurantId,
                PrimaryColor = customization.PrimaryColor,
                SecondaryColor = customization.SecondaryColor,
                TextColor = customization.TextColor,
                BackgroundColor = customization.BackgroundColor,
                FontFamily = customization.FontFamily,
                FontSize = customization.FontSize,
                CustomLogoUrl = customization.CustomLogoUrl,
                FaviconUrl = customization.FaviconUrl,
                SeoTitle = customization.SeoTitle,
                SeoDescription = customization.SeoDescription,
                SeoKeywords = customization.SeoKeywords,
                SocialImageUrl = customization.SocialImageUrl,
                GoogleAnalyticsId = customization.GoogleAnalyticsId,
                CustomCss = customization.CustomCss,
                CustomJs = customization.CustomJs
            },
            Contents = contents
                .Where(c => c.IsVisible)
                .OrderBy(c => c.DisplayOrder)
                .Select(c =>
                {
                    var section = templateSections.FirstOrDefault(s => s.SectionType == c.SectionType);
                    return new WebsiteContentDto
                    {
                        Id = c.Id,
                        RestaurantId = c.RestaurantId,
                        TemplateId = c.TemplateId,
                        SectionType = c.SectionType,
                        Content = c.UseDefaultContent 
                            ? section?.DefaultContent ?? ""
                            : c.Content,
                        UseDefaultContent = c.UseDefaultContent,
                        DisplayOrder = c.DisplayOrder,
                        IsVisible = c.IsVisible,
                        SectionTitle = section?.Title ?? c.SectionType.ToString(),
                        IsRequired = section?.IsRequired ?? false,
                        IsEditable = section?.IsEditable ?? true
                    };
                })
                .ToList()
        };

        return result;
    }
}

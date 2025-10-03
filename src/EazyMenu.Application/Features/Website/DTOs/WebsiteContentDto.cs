using EazyMenu.Domain.Enums;

namespace EazyMenu.Application.Features.Website.DTOs;

/// <summary>
/// DTO محتوای وب‌سایت رستوران
/// </summary>
public class WebsiteContentDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public Guid TemplateId { get; set; }
    public SectionType SectionType { get; set; }
    public string Content { get; set; } = string.Empty;
    public string CustomContent { get; set; } = string.Empty;
    public string DefaultContent { get; set; } = string.Empty;
    public bool UseDefaultContent { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsVisible { get; set; }
    
    // اطلاعات Template Section برای نمایش در View
    public string SectionTitle { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public bool IsEditable { get; set; }
}

/// <summary>
/// DTO کامل وب‌سایت رستوران (برای نمایش عمومی)
/// </summary>
public class RestaurantWebsiteDto
{
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public string RestaurantSlug { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    
    // Template Info
    public WebsiteTemplateDto? Template { get; set; }
    
    // Customization
    public WebsiteCustomizationDto? Customization { get; set; }
    
    // Contents
    public List<WebsiteContentDto> Contents { get; set; } = new();
}

using EazyMenu.Domain.Enums;

namespace EazyMenu.Application.Features.Website.DTOs;

/// <summary>
/// DTO نمایش قالب وب‌سایت
/// </summary>
public class WebsiteTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TemplateType Type { get; set; }
    public string TemplatePath { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string PreviewImageUrl { get; set; } = string.Empty;
    public bool IsFree { get; set; }
    public int DisplayOrder { get; set; }
    public List<TemplateSectionDto> Sections { get; set; } = new();
}

/// <summary>
/// DTO بخش قالب
/// </summary>
public class TemplateSectionDto
{
    public Guid Id { get; set; }
    public SectionType SectionType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string DefaultContent { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public bool IsEditable { get; set; }
    public int DisplayOrder { get; set; }
}

using EazyMenu.Domain.Common;
using EazyMenu.Domain.Enums;

namespace EazyMenu.Domain.Entities;

/// <summary>
/// محتوای وب‌سایت رستوران - محتوای سفارشی شده توسط صاحب رستوران
/// </summary>
public class WebsiteContent : BaseEntity, IAggregateRoot
{
    /// <summary>
    /// شناسه رستوران
    /// </summary>
    public Guid RestaurantId { get; set; }
    
    /// <summary>
    /// شناسه قالب انتخاب شده
    /// </summary>
    public Guid TemplateId { get; set; }
    
    /// <summary>
    /// نوع بخش
    /// </summary>
    public SectionType SectionType { get; set; }
    
    /// <summary>
    /// محتوای HTML سفارشی شده
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// آیا از محتوای پیش‌فرض استفاده شود؟
    /// </summary>
    public bool UseDefaultContent { get; set; } = true;
    
    /// <summary>
    /// ترتیب نمایش
    /// </summary>
    public int DisplayOrder { get; set; } = 0;
    
    /// <summary>
    /// آیا این بخش نمایش داده شود؟
    /// </summary>
    public bool IsVisible { get; set; } = true;
    
    // Navigation
    public virtual Restaurant Restaurant { get; set; } = null!;
    public virtual WebsiteTemplate Template { get; set; } = null!;
}

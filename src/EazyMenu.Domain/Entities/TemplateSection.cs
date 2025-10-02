using EazyMenu.Domain.Common;
using EazyMenu.Domain.Enums;

namespace EazyMenu.Domain.Entities;

/// <summary>
/// بخش‌های قالب - هر قالب از چه بخش‌هایی تشکیل شده
/// </summary>
public class TemplateSection : BaseEntity
{
    /// <summary>
    /// شناسه قالب
    /// </summary>
    public Guid TemplateId { get; set; }
    
    /// <summary>
    /// نوع بخش
    /// </summary>
    public SectionType SectionType { get; set; }
    
    /// <summary>
    /// عنوان بخش (فارسی)
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// عنوان بخش (انگلیسی)
    /// </summary>
    public string TitleEn { get; set; } = string.Empty;
    
    /// <summary>
    /// محتوای پیش‌فرض HTML
    /// </summary>
    public string DefaultContent { get; set; } = string.Empty;
    
    /// <summary>
    /// آیا این بخش الزامی است؟
    /// </summary>
    public bool IsRequired { get; set; } = true;
    
    /// <summary>
    /// آیا قابل ویرایش است؟
    /// </summary>
    public bool IsEditable { get; set; } = true;
    
    /// <summary>
    /// ترتیب نمایش در صفحه
    /// </summary>
    public int DisplayOrder { get; set; } = 0;
    
    // Navigation
    public virtual WebsiteTemplate Template { get; set; } = null!;
}

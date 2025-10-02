using EazyMenu.Domain.Common;
using EazyMenu.Domain.Enums;

namespace EazyMenu.Domain.Entities;

/// <summary>
/// قالب وب‌سایت - Templates از پیش طراحی شده
/// </summary>
public class WebsiteTemplate : BaseEntity
{
    /// <summary>
    /// نام قالب (فارسی)
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// نام قالب (انگلیسی)
    /// </summary>
    public string NameEn { get; set; } = string.Empty;
    
    /// <summary>
    /// توضیحات قالب
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// نوع قالب
    /// </summary>
    public TemplateType Type { get; set; }
    
    /// <summary>
    /// مسیر فایل قالب در wwwroot/templates/
    /// </summary>
    public string TemplatePath { get; set; } = string.Empty;
    
    /// <summary>
    /// آدرس تصویر پیش‌نمایش
    /// </summary>
    public string ThumbnailUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// آدرس تصویر پیش‌نمایش کامل
    /// </summary>
    public string PreviewImageUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// رنگ‌های پیش‌فرض (JSON)
    /// </summary>
    public string DefaultColors { get; set; } = string.Empty;
    
    /// <summary>
    /// فونت‌های پیش‌فرض (JSON)
    /// </summary>
    public string DefaultFonts { get; set; } = string.Empty;
    
    /// <summary>
    /// آیا قالب فعال است؟
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// ترتیب نمایش
    /// </summary>
    public int DisplayOrder { get; set; } = 0;
    
    /// <summary>
    /// آیا قالب رایگان است؟
    /// </summary>
    public bool IsFree { get; set; } = true;
    
    /// <summary>
    /// بخش‌های قابل ویرایش این قالب
    /// </summary>
    public virtual ICollection<TemplateSection> Sections { get; set; } = new List<TemplateSection>();
}

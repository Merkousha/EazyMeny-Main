using EazyMenu.Domain.Enums;

namespace EazyMenu.Web.Areas.Restaurant.Models;

/// <summary>
/// ViewModel ویرایش محتوای بخش وب‌سایت
/// </summary>
public class EditContentViewModel
{
    public Guid RestaurantId { get; set; }
    public Guid? ContentId { get; set; }
    
    // اطلاعات بخش
    public SectionType SectionType { get; set; }
    public string SectionTitle { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public bool IsEditable { get; set; }
    
    // محتوا
    public string CustomContent { get; set; } = string.Empty;
    public string DefaultContent { get; set; } = string.Empty;
    public bool UseDefaultContent { get; set; }
    public bool IsVisible { get; set; } = true;
}

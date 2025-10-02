using EazyMenu.Domain.Common;

namespace EazyMenu.Domain.Entities;

/// <summary>
/// تنظیمات سفارشی‌سازی وب‌سایت رستوران (رنگ، فونت، لوگو، SEO)
/// </summary>
public class WebsiteCustomization : BaseEntity
{
    /// <summary>
    /// شناسه رستوران
    /// </summary>
    public Guid RestaurantId { get; set; }
    
    /// <summary>
    /// رنگ اصلی (Primary Color) - Hex
    /// </summary>
    public string PrimaryColor { get; set; } = "#FF6B35";
    
    /// <summary>
    /// رنگ ثانویه (Secondary Color) - Hex
    /// </summary>
    public string SecondaryColor { get; set; } = "#004E89";
    
    /// <summary>
    /// رنگ متن اصلی - Hex
    /// </summary>
    public string TextColor { get; set; } = "#333333";
    
    /// <summary>
    /// رنگ پس‌زمینه - Hex
    /// </summary>
    public string BackgroundColor { get; set; } = "#FFFFFF";
    
    /// <summary>
    /// فونت اصلی (Font Family)
    /// </summary>
    public string FontFamily { get; set; } = "IRANSans";
    
    /// <summary>
    /// اندازه فونت پیش‌فرض (px)
    /// </summary>
    public int FontSize { get; set; } = 16;
    
    /// <summary>
    /// آدرس لوگو سفارشی
    /// </summary>
    public string? CustomLogoUrl { get; set; }
    
    /// <summary>
    /// آدرس Favicon
    /// </summary>
    public string? FaviconUrl { get; set; }
    
    /// <summary>
    /// عنوان SEO
    /// </summary>
    public string SeoTitle { get; set; } = string.Empty;
    
    /// <summary>
    /// توضیحات SEO
    /// </summary>
    public string SeoDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// کلمات کلیدی SEO
    /// </summary>
    public string SeoKeywords { get; set; } = string.Empty;
    
    /// <summary>
    /// تصویر شبکه‌های اجتماعی (og:image)
    /// </summary>
    public string? SocialImageUrl { get; set; }
    
    /// <summary>
    /// کد Google Analytics
    /// </summary>
    public string? GoogleAnalyticsId { get; set; }
    
    /// <summary>
    /// کدهای سفارشی CSS
    /// </summary>
    public string? CustomCss { get; set; }
    
    /// <summary>
    /// کدهای سفارشی JavaScript
    /// </summary>
    public string? CustomJs { get; set; }
    
    // Navigation
    public virtual Restaurant Restaurant { get; set; } = null!;
}

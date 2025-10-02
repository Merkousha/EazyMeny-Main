namespace EazyMenu.Application.Features.Website.DTOs;

/// <summary>
/// DTO تنظیمات سفارشی‌سازی وب‌سایت
/// </summary>
public class WebsiteCustomizationDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    
    // رنگ‌ها
    public string PrimaryColor { get; set; } = "#FF6B35";
    public string SecondaryColor { get; set; } = "#004E89";
    public string TextColor { get; set; } = "#333333";
    public string BackgroundColor { get; set; } = "#FFFFFF";
    
    // فونت
    public string FontFamily { get; set; } = "IRANSans";
    public int FontSize { get; set; } = 16;
    
    // تصاویر
    public string? CustomLogoUrl { get; set; }
    public string? FaviconUrl { get; set; }
    
    // SEO
    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;
    public string SeoKeywords { get; set; } = string.Empty;
    public string? SocialImageUrl { get; set; }
    
    // Analytics & Custom Code
    public string? GoogleAnalyticsId { get; set; }
    public string? CustomCss { get; set; }
    public string? CustomJs { get; set; }
}

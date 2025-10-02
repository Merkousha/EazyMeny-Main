using MediatR;

namespace EazyMenu.Application.Features.Website.Commands.UpdateCustomization;

/// <summary>
/// Command به‌روزرسانی تنظیمات سفارشی‌سازی وب‌سایت
/// </summary>
public class UpdateCustomizationCommand : IRequest<bool>
{
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
    
    // Analytics
    public string? GoogleAnalyticsId { get; set; }
    
    // Custom Code
    public string? CustomCss { get; set; }
    public string? CustomJs { get; set; }
}

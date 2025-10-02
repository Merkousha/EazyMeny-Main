namespace EazyMenu.Application.Common.Models.Restaurant;

/// <summary>
/// DTO کامل اطلاعات رستوران برای نمایش
/// </summary>
public class RestaurantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ManagerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    
    // ساعات کاری (JSON)
    public string? WorkingHours { get; set; }
    
    // شبکه‌های اجتماعی
    public string? InstagramUrl { get; set; }
    public string? TelegramUrl { get; set; }
    public string? WhatsAppNumber { get; set; }
    
    // تنظیمات
    public bool IsActive { get; set; }
    public bool AcceptOnlineOrders { get; set; }
    public bool AcceptReservations { get; set; }
    
    // QR Code
    public string QRCodeUrl { get; set; } = string.Empty;
    public int QRCodeScanCount { get; set; }
    
    // وب‌سایت اختصاصی
    public string? WebsiteUrl { get; set; }
    public string? WebsiteTheme { get; set; }
    public bool IsWebsitePublished { get; set; }
    public DateTime? WebsitePublishedAt { get; set; }
    
    // هزینه‌ها
    public decimal DeliveryFee { get; set; }
    public decimal MinimumOrderAmount { get; set; }
    
    // مالک
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    
    // تاریخ‌ها
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

using EazyMenu.Domain.Common;
using EazyMenu.Domain.Enums;

namespace EazyMenu.Domain.Entities;

/// <summary>
/// Entity رستوران
/// </summary>
public class Restaurant : BaseEntity, IAggregateRoot
{
    public string Name { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty; // برای URL
    public string Description { get; set; } = string.Empty;
    public string ManagerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    
    // ساعات کاری
    public string? WorkingHours { get; set; } // JSON format
    
    // شبکه‌های اجتماعی
    public string? InstagramUrl { get; set; }
    public string? TelegramUrl { get; set; }
    public string? WhatsAppNumber { get; set; }
    
    // تنظیمات
    public bool IsActive { get; set; } = true;
    public bool AcceptOnlineOrders { get; set; } = true;
    public bool AcceptReservations { get; set; } = false;
    
    // QR Code
    public string QRCodeUrl { get; set; } = string.Empty;
    public int QRCodeScanCount { get; set; } = 0;
    
    // وب‌سایت اختصاصی (US-012)
    public string? WebsiteUrl { get; set; } // myrestaurant.eazymenu.ir
    public string? WebsiteTheme { get; set; } // JSON: {template, colors, fonts, seo}
    public bool IsWebsitePublished { get; set; } = false;
    public DateTime? WebsitePublishedAt { get; set; }
    
    // تنظیمات ارسال (US-009)
    public decimal DeliveryFee { get; set; } = 0;
    public decimal MinimumOrderAmount { get; set; } = 0;
    
    // Relationships
    public Guid OwnerId { get; set; }
    // NOTE: Navigation property removed to maintain Clean Architecture
    // Use OwnerId directly or query through IUserService
    
    public Guid? SubscriptionId { get; set; }
    public virtual Subscription? Subscription { get; set; }
    
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public virtual ICollection<WebsiteContent> WebsiteContents { get; set; } = new List<WebsiteContent>();
    public virtual WebsiteCustomization? WebsiteCustomization { get; set; }
}

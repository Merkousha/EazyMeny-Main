using EazyMenu.Domain.Common;
using EazyMenu.Domain.Enums;

namespace EazyMenu.Domain.Entities;

/// <summary>
/// پلن اشتراک - تعریف بسته‌های مختلف اشتراک
/// </summary>
public class SubscriptionPlan : BaseEntity, IAggregateRoot
{
    /// <summary>
    /// نوع پلن (Basic, Standard, Premium)
    /// </summary>
    public PlanType PlanType { get; set; }
    
    /// <summary>
    /// نام فارسی پلن
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// توضیحات
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// قیمت ماهانه (تومان)
    /// </summary>
    public decimal PriceMonthly { get; set; }
    
    /// <summary>
    /// قیمت سالانه (تومان)
    /// </summary>
    public decimal PriceYearly { get; set; }
    
    /// <summary>
    /// حداکثر تعداد محصولات (-1 = نامحدود)
    /// </summary>
    public int MaxProducts { get; set; }
    
    /// <summary>
    /// حداکثر تعداد دسته‌بندی‌ها (-1 = نامحدود)
    /// </summary>
    public int MaxCategories { get; set; }
    
    /// <summary>
    /// حداکثر تعداد سفارشات در ماه (-1 = نامحدود)
    /// </summary>
    public int MaxOrders { get; set; }
    
    /// <summary>
    /// آیا QR Code دارد؟
    /// </summary>
    public bool HasQRCode { get; set; }
    
    /// <summary>
    /// آیا وب‌سایت اختصاصی دارد؟
    /// </summary>
    public bool HasWebsite { get; set; }
    
    /// <summary>
    /// آیا سیستم رزرو دارد؟
    /// </summary>
    public bool HasReservation { get; set; }
    
    /// <summary>
    /// آیا گزارش‌گیری پیشرفته دارد؟
    /// </summary>
    public bool HasAnalytics { get; set; }
    
    /// <summary>
    /// سطح پشتیبانی (ایمیل، تلفنی، 24/7)
    /// </summary>
    public string SupportLevel { get; set; } = string.Empty;
    
    /// <summary>
    /// ویژگی‌های پلن (JSON)
    /// </summary>
    public string Features { get; set; } = "[]";
    
    /// <summary>
    /// ترتیب نمایش
    /// </summary>
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// آیا پلن فعال است؟
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// آیا پلن محبوب است؟ (برای نمایش Badge)
    /// </summary>
    public bool IsPopular { get; set; }
    
    // Navigation property
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}

using EazyMenu.Domain.Common;
using EazyMenu.Domain.Enums;

namespace EazyMenu.Domain.Entities;

/// <summary>
/// Entity اشتراک
/// </summary>
public class Subscription : BaseEntity
{
    public Guid RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; set; } = null!;
    
    /// <summary>
    /// شناسه پلن اشتراک
    /// </summary>
    public Guid SubscriptionPlanId { get; set; }
    public virtual SubscriptionPlan SubscriptionPlan { get; set; } = null!;
    
    public SubscriptionStatus Status { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    // قیمت‌گذاری
    public decimal Amount { get; set; }
    public bool IsYearly { get; set; } = false; // ماهانه یا سالانه
    
    // تمدید خودکار
    public bool AutoRenew { get; set; } = false;
    public Guid? PaymentMethodId { get; set; }
    
    // محدودیت‌ها (بر اساس پلن)
    public int MaxProducts { get; set; }
    public int MaxOrdersPerMonth { get; set; }
    public bool HasReservationFeature { get; set; }
    public bool HasWebsiteBuilder { get; set; }
    public bool HasAdvancedReporting { get; set; }
    
    // استفاده فعلی
    public int CurrentProductCount { get; set; } = 0;
    public int CurrentMonthOrderCount { get; set; } = 0;
    
    // Relationships
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

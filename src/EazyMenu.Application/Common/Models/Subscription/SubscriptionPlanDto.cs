namespace EazyMenu.Application.Common.Models.Subscription;

/// <summary>
/// مدل نمایش پلن اشتراک
/// </summary>
public class SubscriptionPlanDto
{
    public string PlanType { get; set; } = string.Empty; // Basic, Standard, Premium
    public string Name { get; set; } = string.Empty;
    public decimal PriceMonthly { get; set; }
    public decimal PriceYearly { get; set; }
    public int MaxProducts { get; set; }
    public int MaxCategories { get; set; }
    public int MaxOrders { get; set; }
    public bool HasQRCode { get; set; }
    public bool HasWebsite { get; set; }
    public bool HasReservation { get; set; }
    public bool HasAnalytics { get; set; }
    public string SupportLevel { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new();
    
    /// <summary>
    /// محاسبه درصد تخفیف سالانه
    /// </summary>
    public decimal YearlyDiscountPercentage => 
        PriceMonthly > 0 
            ? Math.Round(((PriceMonthly * 12 - PriceYearly) / (PriceMonthly * 12)) * 100, 0) 
            : 0;
    
    /// <summary>
    /// آیا محصولات نامحدود است؟
    /// </summary>
    public bool IsUnlimitedProducts => MaxProducts == -1;
    
    /// <summary>
    /// آیا دسته‌بندی‌ها نامحدود است؟
    /// </summary>
    public bool IsUnlimitedCategories => MaxCategories == -1;
    
    /// <summary>
    /// آیا سفارشات نامحدود است؟
    /// </summary>
    public bool IsUnlimitedOrders => MaxOrders == -1;
}

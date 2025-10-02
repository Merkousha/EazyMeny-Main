namespace EazyMenu.Domain.Enums;

/// <summary>
/// وضعیت اشتراک
/// </summary>
public enum SubscriptionStatus
{
    Trial = 0,              // دوره آزمایشی
    Active = 1,             // فعال
    Expiring = 2,           // در حال انقضا (کمتر از 7 روز)
    Expired = 3,            // منقضی شده
    Suspended = 4,          // معلق
    Cancelled = 5           // لغو شده
}

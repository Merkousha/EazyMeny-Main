namespace EazyMenu.Domain.Enums;

/// <summary>
/// وضعیت‌های سفارش
/// </summary>
public enum OrderStatus
{
    Pending = 0,           // در انتظار تایید
    Confirmed = 1,         // تایید شده
    Preparing = 2,         // در حال آماده‌سازی
    Ready = 3,             // آماده تحویل
    Delivered = 4,         // تحویل داده شده
    Cancelled = 5          // لغو شده
}

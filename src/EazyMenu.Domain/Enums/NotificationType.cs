namespace EazyMenu.Domain.Enums;

/// <summary>
/// انواع نوتیفیکیشن
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// پیامک
    /// </summary>
    Sms = 1,
    
    /// <summary>
    /// ایمیل
    /// </summary>
    Email = 2,
    
    /// <summary>
    /// اعلان درون برنامه‌ای
    /// </summary>
    InApp = 3,
    
    /// <summary>
    /// تایید سفارش
    /// </summary>
    OrderConfirmation = 10,
    
    /// <summary>
    /// آماده شدن سفارش
    /// </summary>
    OrderReady = 11,
    
    /// <summary>
    /// سفارش تحویل داده شد
    /// </summary>
    OrderDelivered = 12,
    
    /// <summary>
    /// یادآوری رزرو
    /// </summary>
    ReservationReminder = 20,
    
    /// <summary>
    /// تایید رزرو
    /// </summary>
    ReservationConfirmed = 21,
    
    /// <summary>
    /// در حال انقضای اشتراک
    /// </summary>
    SubscriptionExpiring = 30,
    
    /// <summary>
    /// اشتراک منقضی شد
    /// </summary>
    SubscriptionExpired = 31
}

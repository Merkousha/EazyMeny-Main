namespace EazyMenu.Domain.Enums;

/// <summary>
/// وضعیت رزرو میز
/// </summary>
public enum ReservationStatus
{
    /// <summary>
    /// در انتظار تایید
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// تایید شده
    /// </summary>
    Confirmed = 1,
    
    /// <summary>
    /// مشتری حاضر شده
    /// </summary>
    CheckedIn = 2,
    
    /// <summary>
    /// رزرو تکمیل شده
    /// </summary>
    Completed = 3,
    
    /// <summary>
    /// لغو شده توسط مشتری یا رستوران
    /// </summary>
    Cancelled = 4,
    
    /// <summary>
    /// عدم حضور مشتری (No-Show)
    /// </summary>
    NoShow = 5
}

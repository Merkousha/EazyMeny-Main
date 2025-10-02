namespace EazyMenu.Domain.Enums;

/// <summary>
/// نقش‌های کاربران
/// </summary>
public enum UserRole
{
    SuperAdmin = 1,         // ادمین اصلی
    FinanceAdmin = 2,       // ادمین مالی
    SupportAdmin = 3,       // ادمین پشتیبانی
    ContentManager = 4,     // مدیر محتوا
    RestaurantOwner = 10,   // صاحب رستوران
    RestaurantManager = 11, // مدیر رستوران
    RestaurantStaff = 12    // کارمند رستوران
}

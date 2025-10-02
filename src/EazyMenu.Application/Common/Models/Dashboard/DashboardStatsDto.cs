namespace EazyMenu.Application.Common.Models.Dashboard;

/// <summary>
/// DTO برای نمایش آمار کلی در داشبورد ادمین
/// </summary>
public class DashboardStatsDto
{
    /// <summary>
    /// تعداد کل رستوران‌ها
    /// </summary>
    public int TotalRestaurants { get; set; }
    
    /// <summary>
    /// تعداد رستوران‌های فعال
    /// </summary>
    public int ActiveRestaurants { get; set; }
    
    /// <summary>
    /// تعداد کل دسته‌بندی‌ها
    /// </summary>
    public int TotalCategories { get; set; }
    
    /// <summary>
    /// تعداد کل محصولات
    /// </summary>
    public int TotalProducts { get; set; }
    
    /// <summary>
    /// تعداد محصولات فعال
    /// </summary>
    public int ActiveProducts { get; set; }
    
    /// <summary>
    /// تعداد کل کاربران
    /// </summary>
    public int TotalUsers { get; set; }
    
    /// <summary>
    /// تعداد رستوران‌های ثبت شده امروز
    /// </summary>
    public int TodayRestaurants { get; set; }
    
    /// <summary>
    /// تعداد رستوران‌های ثبت شده این هفته
    /// </summary>
    public int WeekRestaurants { get; set; }
    
    /// <summary>
    /// تعداد رستوران‌های ثبت شده این ماه
    /// </summary>
    public int MonthRestaurants { get; set; }
}

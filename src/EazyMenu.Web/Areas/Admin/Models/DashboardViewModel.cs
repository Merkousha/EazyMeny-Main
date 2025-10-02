using EazyMenu.Application.Common.Models.Dashboard;
using EazyMenu.Application.Common.Models.Restaurant;

namespace EazyMenu.Web.Areas.Admin.Models;

/// <summary>
/// ViewModel برای نمایش داشبورد ادمین
/// </summary>
public class DashboardViewModel
{
    /// <summary>
    /// آمار کلی سیستم
    /// </summary>
    public DashboardStatsDto Stats { get; set; } = new();

    /// <summary>
    /// آخرین رستوران‌های ثبت شده
    /// </summary>
    public List<RestaurantListDto> RecentRestaurants { get; set; } = new();
}

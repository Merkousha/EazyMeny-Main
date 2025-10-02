using EazyMenu.Domain.Enums;

namespace EazyMenu.Application.Common.Models.Order;

/// <summary>
/// DTO لیست سفارش‌ها برای پنل ادمین
/// </summary>
public class OrderListDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string RestaurantName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    
    /// <summary>
    /// وضعیت سفارش (Enum)
    /// </summary>
    public OrderStatus Status { get; set; }
    
    /// <summary>
    /// متن وضعیت فارسی
    /// </summary>
    public string StatusText => Status switch
    {
        OrderStatus.Pending => "در انتظار تایید",
        OrderStatus.Confirmed => "تایید شده",
        OrderStatus.Preparing => "در حال آماده‌سازی",
        OrderStatus.Ready => "آماده تحویل",
        OrderStatus.Delivered => "تحویل داده شده",
        OrderStatus.Cancelled => "لغو شده",
        _ => "نامشخص"
    };
    
    /// <summary>
    /// کلاس CSS برای Badge وضعیت
    /// </summary>
    public string StatusBadgeClass => Status switch
    {
        OrderStatus.Pending => "bg-warning",
        OrderStatus.Confirmed => "bg-info",
        OrderStatus.Preparing => "bg-primary",
        OrderStatus.Ready => "bg-success",
        OrderStatus.Delivered => "bg-secondary",
        OrderStatus.Cancelled => "bg-danger",
        _ => "bg-dark"
    };
    
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
    public bool IsDelivery { get; set; }
    public int? TableNumber { get; set; }
    public bool IsPaid { get; set; }
    public bool IsOnlinePayment { get; set; }
    
    /// <summary>
    /// تعداد آیتم‌های سفارش
    /// </summary>
    public int ItemsCount { get; set; }
    
    /// <summary>
    /// زمان تحویل مورد نظر
    /// </summary>
    public DateTime? DesiredDeliveryTime { get; set; }
}

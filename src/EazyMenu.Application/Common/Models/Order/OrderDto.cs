using EazyMenu.Domain.Enums;

namespace EazyMenu.Application.Common.Models.Order;

/// <summary>
/// DTO برای نمایش جزئیات سفارش
/// </summary>
public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    
    // اطلاعات رستوران
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public string RestaurantPhone { get; set; } = string.Empty;
    
    // اطلاعات مشتری
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    
    // جزئیات سفارش
    public List<OrderItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal FinalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    
    // وضعیت
    public OrderStatus Status { get; set; }
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
    
    // زمان‌ها
    public DateTime CreatedAt { get; set; }
    public DateTime? PreferredDeliveryTime { get; set; }
    public DateTime? DeliveredAt { get; set; }
    
    // یادداشت
    public string? Note { get; set; }
    
    // نوع سفارش
    public bool IsTakeaway { get; set; }
    public string DeliveryTypeText => IsTakeaway ? "حضوری" : "پیک";
    
    // پرداخت
    public bool IsPaid { get; set; }
    public string? PaymentTrackingCode { get; set; }
}

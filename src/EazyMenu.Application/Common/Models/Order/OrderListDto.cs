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
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
    public bool IsDelivery { get; set; }
    public int? TableNumber { get; set; }
    public bool IsPaid { get; set; }
    public bool IsOnlinePayment { get; set; }
}

namespace EazyMenu.Application.Common.Models.Order;

/// <summary>
/// DTO جزئیات سفارش برای پنل ادمین
/// </summary>
public class OrderDetailsDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string RestaurantName { get; set; } = string.Empty;
    public Guid RestaurantId { get; set; }

    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public bool IsDelivery { get; set; }
    public string? DeliveryAddress { get; set; }
    public int? TableNumber { get; set; }

    public DateTime OrderDate { get; set; }
    public DateTime? DesiredDeliveryTime { get; set; }
    public DateTime? PreparedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }

    public string Status { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal Tax { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }

    public bool IsPaid { get; set; }
    public bool IsOnlinePayment { get; set; }
    public string PaymentStatus => IsPaid ? "پرداخت شده" : "در انتظار پرداخت";

    public string? CustomerNotes { get; set; }
    public string? CancellationReason { get; set; }

    public List<OrderItemDto> Items { get; set; } = new();
}

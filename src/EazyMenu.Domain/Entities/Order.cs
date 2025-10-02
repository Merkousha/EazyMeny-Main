using EazyMenu.Domain.Common;
using EazyMenu.Domain.Enums;

namespace EazyMenu.Domain.Entities;

/// <summary>
/// Entity سفارش
/// </summary>
public class Order : BaseEntity, IAggregateRoot
{
    public string OrderNumber { get; set; } = string.Empty; // مثلاً EZ-12345
    
    public Guid RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; set; } = null!;
    
    // مشتری (ممکن است Guest باشد)
    public Guid? CustomerId { get; set; }
    public virtual ApplicationUser? Customer { get; set; }
    
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    
    // نوع سفارش
    public bool IsDelivery { get; set; } = false; // true: دلیوری, false: حضوری
    public string? DeliveryAddress { get; set; }
    
    // شماره میز (برای سفارش حضوری با QR Code)
    public int? TableNumber { get; set; }
    
    // زمان
    public DateTime OrderDate { get; set; }
    public DateTime? DesiredDeliveryTime { get; set; }
    public DateTime? PreparedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    
    // وضعیت
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    
    // مبلغ
    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; } = 0;
    public decimal Tax { get; set; } = 0;
    public decimal Discount { get; set; } = 0;
    public decimal TotalAmount { get; set; }
    
    // پرداخت
    public bool IsPaid { get; set; } = false;
    public bool IsOnlinePayment { get; set; } = false; // true: آنلاین, false: نقدی
    public Guid? PaymentId { get; set; }
    public virtual Payment? Payment { get; set; }
    
    // توضیحات
    public string? CustomerNotes { get; set; }
    public string? CancellationReason { get; set; }
    
    // Relationships
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

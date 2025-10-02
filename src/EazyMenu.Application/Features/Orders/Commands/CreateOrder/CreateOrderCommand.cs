using MediatR;

namespace EazyMenu.Application.Features.Orders.Commands.CreateOrder;

/// <summary>
/// Command برای ایجاد سفارش جدید از سبد خرید
/// </summary>
public class CreateOrderCommand : IRequest<Guid>
{
    /// <summary>
    /// ID رستوران
    /// </summary>
    public Guid RestaurantId { get; set; }
    
    /// <summary>
    /// نام مشتری
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;
    
    /// <summary>
    /// شماره تماس مشتری
    /// </summary>
    public string CustomerPhone { get; set; } = string.Empty;
    
    /// <summary>
    /// آدرس تحویل
    /// </summary>
    public string DeliveryAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// یادداشت سفارش
    /// </summary>
    public string? Note { get; set; }
    
    /// <summary>
    /// زمان ترجیحی تحویل
    /// </summary>
    public DateTime? PreferredDeliveryTime { get; set; }
    
    /// <summary>
    /// آیا سفارش حضوری است؟
    /// </summary>
    public bool IsTakeaway { get; set; } = false;
    
    /// <summary>
    /// آیتم‌های سفارش (از سبد خرید)
    /// </summary>
    public List<OrderItemCommand> Items { get; set; } = new();
}

/// <summary>
/// آیتم سفارش
/// </summary>
public class OrderItemCommand
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public string? ImageUrl { get; set; }
}

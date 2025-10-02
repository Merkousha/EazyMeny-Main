using EazyMenu.Domain.Common;

namespace EazyMenu.Domain.Entities;

/// <summary>
/// Entity آیتم سفارش
/// </summary>
public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;
    
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    
    public string ProductName { get; set; } = string.Empty; // ذخیره نام برای تاریخچه
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    
    // گزینه‌های انتخاب شده (JSON format)
    public string? SelectedOptions { get; set; } // مثلاً: {"size":"large", "spicy":"medium"}
    
    public decimal TotalPrice { get; set; }
    
    public string? SpecialInstructions { get; set; }
}

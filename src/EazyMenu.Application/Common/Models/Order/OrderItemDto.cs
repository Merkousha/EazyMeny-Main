namespace EazyMenu.Application.Common.Models.Order;

/// <summary>
/// DTO آیتم‌های سفارش
/// </summary>
public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public string? ImageUrl { get; set; }
    public string? SelectedOptions { get; set; }
    public string? SpecialInstructions { get; set; }
}

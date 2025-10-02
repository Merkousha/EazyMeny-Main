namespace EazyMenu.Application.Common.Models.Cart;

/// <summary>
/// DTO آیتم سبد خرید
/// </summary>
public class CartItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }
    public int PreparationTime { get; set; }
    
    /// <summary>
    /// قیمت نهایی (با تخفیف یا بدون تخفیف)
    /// </summary>
    public decimal FinalPrice => DiscountedPrice ?? Price;
    
    /// <summary>
    /// جمع قیمت این آیتم (قیمت × تعداد)
    /// </summary>
    public decimal Subtotal => FinalPrice * Quantity;
    
    /// <summary>
    /// درصد تخفیف (در صورت وجود)
    /// </summary>
    public int? DiscountPercentage
    {
        get
        {
            if (DiscountedPrice.HasValue && Price > 0)
            {
                return (int)Math.Round(((Price - DiscountedPrice.Value) / Price) * 100);
            }
            return null;
        }
    }
}

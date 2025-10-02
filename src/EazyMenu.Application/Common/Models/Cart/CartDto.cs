namespace EazyMenu.Application.Common.Models.Cart;

/// <summary>
/// DTO سبد خرید
/// </summary>
public class CartDto
{
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public string RestaurantSlug { get; set; } = string.Empty;
    public List<CartItemDto> Items { get; set; } = new();
    
    /// <summary>
    /// تعداد کل آیتم‌ها در سبد
    /// </summary>
    public int TotalItems => Items.Sum(i => i.Quantity);
    
    /// <summary>
    /// جمع قیمت بدون تخفیف
    /// </summary>
    public decimal TotalPrice => Items.Sum(i => i.Price * i.Quantity);
    
    /// <summary>
    /// جمع تخفیف
    /// </summary>
    public decimal TotalDiscount => Items.Sum(i => 
        (i.Price - i.FinalPrice) * i.Quantity);
    
    /// <summary>
    /// جمع نهایی (با تخفیف)
    /// </summary>
    public decimal TotalAmount => Items.Sum(i => i.Subtotal);
    
    /// <summary>
    /// زمان تقریبی آماده‌سازی (بیشترین زمان محصولات)
    /// </summary>
    public int EstimatedPreparationTime => Items.Any() 
        ? Items.Max(i => i.PreparationTime) 
        : 0;
}

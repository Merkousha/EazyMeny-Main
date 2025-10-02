namespace EazyMenu.Application.Common.Models.Menu;

/// <summary>
/// DTO محصول در منوی عمومی
/// </summary>
public class ProductMenuDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public string? Image1Url { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsNew { get; set; }
    public bool IsPopular { get; set; }
    public bool IsSpicy { get; set; }
    public bool IsVegetarian { get; set; }
    public int PreparationTime { get; set; }
    
    // محاسبه شده
    public decimal FinalPrice => DiscountedPrice ?? Price;
    public int? DiscountPercentage => DiscountedPrice.HasValue && Price > 0
        ? (int)Math.Round(((Price - DiscountedPrice.Value) / Price) * 100)
        : null;
}

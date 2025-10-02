namespace EazyMenu.Application.Common.Models.Product;

/// <summary>
/// DTO برای نمایش اطلاعات محصول
/// </summary>
public class ProductDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    
    public string? Image1Url { get; set; }
    public string? Image2Url { get; set; }
    public string? Image3Url { get; set; }
    
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public bool IsAvailable { get; set; }
    
    public bool IsNew { get; set; }
    public bool IsPopular { get; set; }
    public bool IsSpicy { get; set; }
    public bool IsVegetarian { get; set; }
    
    public int? StockQuantity { get; set; }
    public int PreparationTime { get; set; }
    
    public string? Options { get; set; }
    public string? NutritionalInfo { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// قیمت نهایی (با تخفیف یا بدون تخفیف)
    /// </summary>
    public decimal FinalPrice => DiscountedPrice ?? Price;
    
    /// <summary>
    /// درصد تخفیف
    /// </summary>
    public int? DiscountPercentage => DiscountedPrice.HasValue 
        ? (int)Math.Round(((Price - DiscountedPrice.Value) / Price) * 100) 
        : null;
}

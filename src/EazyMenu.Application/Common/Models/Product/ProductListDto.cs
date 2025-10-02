namespace EazyMenu.Application.Common.Models.Product;

/// <summary>
/// DTO برای لیست محصولات (سبک‌تر)
/// </summary>
public class ProductListDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    
    public string? Image1Url { get; set; }
    
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public bool IsAvailable { get; set; }
    
    public bool IsNew { get; set; }
    public bool IsPopular { get; set; }
    
    public int? StockQuantity { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// قیمت نهایی (با تخفیف یا بدون تخفیف)
    /// </summary>
    public decimal FinalPrice => DiscountedPrice ?? Price;
    
    /// <summary>
    /// وضعیت موجودی
    /// </summary>
    public string StockStatus
    {
        get
        {
            if (!IsAvailable) return "ناموجود";
            if (StockQuantity == null) return "موجود";
            if (StockQuantity == 0) return "ناموجود";
            if (StockQuantity <= 5) return "کمبود موجودی";
            return "موجود";
        }
    }
}

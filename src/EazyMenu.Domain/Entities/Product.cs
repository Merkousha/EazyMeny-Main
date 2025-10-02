using EazyMenu.Domain.Common;

namespace EazyMenu.Domain.Entities;

/// <summary>
/// Entity محصول (منوی غذا)
/// </summary>
public class Product : BaseEntity
{
    public Guid RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; set; } = null!;
    
    public Guid CategoryId { get; set; }
    public virtual Category Category { get; set; } = null!;
    
    public string Name { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    
    // تصاویر (حداکثر 3 تصویر)
    public string? Image1Url { get; set; }
    public string? Image2Url { get; set; }
    public string? Image3Url { get; set; }
    
    // زمان آماده‌سازی (به دقیقه)
    public int PreparationTime { get; set; } = 30;
    
    // برچسب‌ها
    public bool IsVegetarian { get; set; } = false;
    public bool IsSpicy { get; set; } = false;
    public bool IsPopular { get; set; } = false;
    public bool IsNew { get; set; } = false;
    
    // موجودی
    public bool IsAvailable { get; set; } = true;
    public int? StockQuantity { get; set; } // null = نامحدود
    
    // نمایش
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    
    // گزینه‌ها (JSON format)
    public string? Options { get; set; } // مثلاً سایز، افزودنی‌ها
    
    // اطلاعات تغذیه‌ای (JSON format - اختیاری)
    public string? NutritionalInfo { get; set; }
    
    // Relationships
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

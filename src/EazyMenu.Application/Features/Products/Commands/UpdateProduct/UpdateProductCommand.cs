using MediatR;

namespace EazyMenu.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// Command برای ویرایش محصول
/// </summary>
public class UpdateProductCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public int? PreparationTime { get; set; }
    public bool IsVegetarian { get; set; }
    public bool IsSpicy { get; set; }
    public bool IsPopular { get; set; }
    public bool IsNew { get; set; }
    public bool IsAvailable { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    
    // تصاویر
    public string? Image1Url { get; set; }
    public string? Image2Url { get; set; }
    public string? Image3Url { get; set; }
    
    // موجودی و گزینه‌ها
    public int StockQuantity { get; set; }
    public string? Options { get; set; }
    public string? NutritionalInfo { get; set; }
    
    // برای تصویر تولید شده با AI
    public string? GeneratedImageUrl { get; set; }
}

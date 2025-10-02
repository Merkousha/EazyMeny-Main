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
    public int PreparationTime { get; set; } = 30;
    public string? Options { get; set; }
    public string? NutritionalInfo { get; set; }
}

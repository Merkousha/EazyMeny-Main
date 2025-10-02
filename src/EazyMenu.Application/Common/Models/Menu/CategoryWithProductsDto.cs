namespace EazyMenu.Application.Common.Models.Menu;

/// <summary>
/// DTO دسته‌بندی با محصولات در منوی عمومی
/// </summary>
public class CategoryWithProductsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public int DisplayOrder { get; set; }
    public List<ProductMenuDto> Products { get; set; } = new();
}

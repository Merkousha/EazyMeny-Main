namespace EazyMenu.Application.Common.Models.Menu;

/// <summary>
/// DTO منوی کامل رستوران برای نمایش عمومی
/// </summary>
public class RestaurantMenuDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? WorkingHours { get; set; }
    public bool IsActive { get; set; }
    public List<CategoryWithProductsDto> Categories { get; set; } = new();
}

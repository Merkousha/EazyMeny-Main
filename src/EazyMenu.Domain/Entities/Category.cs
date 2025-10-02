using EazyMenu.Domain.Common;

namespace EazyMenu.Domain.Entities;

/// <summary>
/// Entity دسته‌بندی منو
/// </summary>
public class Category : BaseEntity
{
    public Guid RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; set; } = null!;
    
    public string Name { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Relationships
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

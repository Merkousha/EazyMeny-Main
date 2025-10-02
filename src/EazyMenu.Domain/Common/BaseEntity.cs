namespace EazyMenu.Domain.Common;

/// <summary>
/// کلاس پایه برای تمام Entity ها
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } // Soft Delete
}

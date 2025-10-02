using MediatR;

namespace EazyMenu.Application.Features.Categories.Commands.CreateCategory;

/// <summary>
/// Command برای ایجاد دسته‌بندی جدید
/// </summary>
public class CreateCategoryCommand : IRequest<Guid>
{
    public Guid RestaurantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}

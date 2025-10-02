using MediatR;

namespace EazyMenu.Application.Features.Categories.Commands.UpdateCategory;

/// <summary>
/// Command برای ویرایش دسته‌بندی
/// </summary>
public class UpdateCategoryCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

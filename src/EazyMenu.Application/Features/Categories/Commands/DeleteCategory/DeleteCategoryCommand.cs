using MediatR;

namespace EazyMenu.Application.Features.Categories.Commands.DeleteCategory;

/// <summary>
/// Command برای حذف دسته‌بندی (Soft Delete)
/// </summary>
public class DeleteCategoryCommand : IRequest<bool>
{
    public Guid Id { get; set; }

    public DeleteCategoryCommand(Guid id)
    {
        Id = id;
    }
}

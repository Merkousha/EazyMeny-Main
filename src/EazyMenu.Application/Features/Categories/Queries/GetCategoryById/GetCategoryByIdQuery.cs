using EazyMenu.Application.Common.Models.Category;
using MediatR;

namespace EazyMenu.Application.Features.Categories.Queries.GetCategoryById;

/// <summary>
/// Query برای دریافت اطلاعات یک دسته‌بندی
/// </summary>
public class GetCategoryByIdQuery : IRequest<CategoryDto?>
{
    public Guid Id { get; set; }

    public GetCategoryByIdQuery(Guid id)
    {
        Id = id;
    }
}

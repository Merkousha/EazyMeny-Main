using EazyMenu.Application.Common.Models.Product;
using MediatR;

namespace EazyMenu.Application.Features.Products.Queries.GetProductsByCategory;

/// <summary>
/// Query برای دریافت محصولات یک دسته‌بندی
/// </summary>
public class GetProductsByCategoryQuery : IRequest<List<ProductListDto>>
{
    public Guid CategoryId { get; set; }

    public GetProductsByCategoryQuery(Guid categoryId)
    {
        CategoryId = categoryId;
    }
}

using EazyMenu.Application.Common.Models.Product;
using MediatR;

namespace EazyMenu.Application.Features.Products.Queries.GetProductById;

/// <summary>
/// Query برای دریافت اطلاعات یک محصول
/// </summary>
public class GetProductByIdQuery : IRequest<ProductDto?>
{
    public Guid Id { get; set; }

    public GetProductByIdQuery(Guid id)
    {
        Id = id;
    }
}

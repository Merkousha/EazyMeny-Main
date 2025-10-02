using EazyMenu.Application.Common.Models.Product;
using MediatR;

namespace EazyMenu.Application.Features.Products.Queries.GetAllProducts;

/// <summary>
/// Query برای دریافت تمام محصولات (برای ادمین)
/// </summary>
public class GetAllProductsQuery : IRequest<List<ProductListDto>>
{
    // می‌توان پارامترهای Filter و Pagination اضافه کرد
}

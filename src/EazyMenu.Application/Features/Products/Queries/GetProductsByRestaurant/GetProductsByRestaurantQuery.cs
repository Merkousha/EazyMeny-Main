using EazyMenu.Application.Common.Models.Product;
using MediatR;

namespace EazyMenu.Application.Features.Products.Queries.GetProductsByRestaurant;

/// <summary>
/// Query برای دریافت محصولات یک رستوران
/// </summary>
public class GetProductsByRestaurantQuery : IRequest<List<ProductListDto>>
{
    public Guid RestaurantId { get; set; }

    public GetProductsByRestaurantQuery(Guid restaurantId)
    {
        RestaurantId = restaurantId;
    }
}

using EazyMenu.Application.Common.Models.Restaurant;
using MediatR;

namespace EazyMenu.Application.Features.Restaurants.Queries.GetRestaurantById;

/// <summary>
/// Query برای دریافت اطلاعات یک رستوران
/// </summary>
public class GetRestaurantByIdQuery : IRequest<RestaurantDto?>
{
    public Guid Id { get; set; }

    public GetRestaurantByIdQuery(Guid id)
    {
        Id = id;
    }
}

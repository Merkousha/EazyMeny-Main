using EazyMenu.Application.Common.Models.Restaurant;
using MediatR;

namespace EazyMenu.Application.Features.Restaurants.Queries.GetRestaurantsByOwner;

/// <summary>
/// Query برای دریافت لیست رستوران‌های یک صاحب
/// </summary>
public class GetRestaurantsByOwnerQuery : IRequest<List<RestaurantListDto>>
{
    public Guid OwnerId { get; set; }

    public GetRestaurantsByOwnerQuery(Guid ownerId)
    {
        OwnerId = ownerId;
    }
}

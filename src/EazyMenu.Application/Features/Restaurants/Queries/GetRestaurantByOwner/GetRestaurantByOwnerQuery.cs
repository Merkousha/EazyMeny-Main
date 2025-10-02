using MediatR;
using EazyMenu.Application.Common.Models;

namespace EazyMenu.Application.Features.Restaurants.Queries.GetRestaurantByOwner;

/// <summary>
/// Query برای دریافت رستوران بر اساس صاحب رستوران
/// </summary>
public class GetRestaurantByOwnerQuery : IRequest<RestaurantBasicDto?>
{
    public Guid OwnerId { get; set; }
}

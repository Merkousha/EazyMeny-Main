using EazyMenu.Application.Common.Models.Restaurant;
using MediatR;

namespace EazyMenu.Application.Features.Restaurants.Queries.GetRestaurantBySlug;

/// <summary>
/// Query برای دریافت رستوران با Slug
/// </summary>
public record GetRestaurantBySlugQuery : IRequest<RestaurantDto?>
{
    public string Slug { get; init; } = string.Empty;
}

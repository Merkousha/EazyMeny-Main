using EazyMenu.Application.Common.Models.Restaurant;
using MediatR;

namespace EazyMenu.Application.Features.Restaurants.Queries.GetAllRestaurants;

/// <summary>
/// Query برای دریافت تمام رستوران‌ها (برای ادمین)
/// </summary>
public class GetAllRestaurantsQuery : IRequest<List<RestaurantListDto>>
{
    // می‌توان پارامترهای Filter و Pagination اضافه کرد
}

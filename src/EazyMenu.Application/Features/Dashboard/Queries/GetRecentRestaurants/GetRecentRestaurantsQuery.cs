using EazyMenu.Application.Common.Models.Restaurant;
using MediatR;

namespace EazyMenu.Application.Features.Dashboard.Queries.GetRecentRestaurants;

/// <summary>
/// Query برای دریافت آخرین رستوران‌های ثبت شده
/// </summary>
public class GetRecentRestaurantsQuery : IRequest<List<RestaurantListDto>>
{
    /// <summary>
    /// تعداد رستوران‌هایی که باید بازگردانده شود
    /// </summary>
    public int Limit { get; set; }

    public GetRecentRestaurantsQuery(int limit = 5)
    {
        Limit = limit;
    }
}

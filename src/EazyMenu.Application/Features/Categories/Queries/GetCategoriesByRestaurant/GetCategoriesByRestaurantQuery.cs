using EazyMenu.Application.Common.Models.Category;
using MediatR;

namespace EazyMenu.Application.Features.Categories.Queries.GetCategoriesByRestaurant;

/// <summary>
/// Query برای دریافت دسته‌بندی‌های یک رستوران
/// </summary>
public class GetCategoriesByRestaurantQuery : IRequest<List<CategoryListDto>>
{
    public Guid RestaurantId { get; set; }

    public GetCategoriesByRestaurantQuery(Guid restaurantId)
    {
        RestaurantId = restaurantId;
    }
}

using EazyMenu.Application.Features.Website.DTOs;
using MediatR;

namespace EazyMenu.Application.Features.Website.Queries.GetRestaurantWebsite;

/// <summary>
/// Query دریافت اطلاعات کامل وب‌سایت رستوران
/// </summary>
public class GetRestaurantWebsiteQuery : IRequest<RestaurantWebsiteDto?>
{
    public Guid RestaurantId { get; set; }
    public string? RestaurantSlug { get; set; }
    public bool OnlyPublished { get; set; } = true;
    public bool IncludeHiddenSections { get; set; } = false;
}

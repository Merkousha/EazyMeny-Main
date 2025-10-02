using EazyMenu.Application.Common.Models.Menu;
using MediatR;

namespace EazyMenu.Application.Features.Menu.Queries.GetMenuBySlug;

/// <summary>
/// Query برای دریافت منوی عمومی رستوران با Slug
/// </summary>
public record GetMenuBySlugQuery(string Slug) : IRequest<RestaurantMenuDto?>;

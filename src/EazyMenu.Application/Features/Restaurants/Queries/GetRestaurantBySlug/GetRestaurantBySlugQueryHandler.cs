using AutoMapper;
using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Restaurant;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Restaurants.Queries.GetRestaurantBySlug;

/// <summary>
/// Handler برای دریافت رستوران با Slug
/// </summary>
public class GetRestaurantBySlugQueryHandler : IRequestHandler<GetRestaurantBySlugQuery, RestaurantDto?>
{
    private readonly IRepository<Restaurant> _repository;
    private readonly IMapper _mapper;

    public GetRestaurantBySlugQueryHandler(IRepository<Restaurant> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<RestaurantDto?> Handle(GetRestaurantBySlugQuery request, CancellationToken cancellationToken)
    {
        var restaurant = (await _repository.GetAllAsync(cancellationToken))
            .FirstOrDefault(r => r.Slug == request.Slug && r.IsActive);

        if (restaurant == null)
            return null;

        return _mapper.Map<RestaurantDto>(restaurant);
    }
}

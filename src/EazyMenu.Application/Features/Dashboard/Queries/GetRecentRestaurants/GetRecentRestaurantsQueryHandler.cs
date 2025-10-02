using AutoMapper;
using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Restaurant;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Dashboard.Queries.GetRecentRestaurants;

/// <summary>
/// Handler برای دریافت آخرین رستوران‌های ثبت شده
/// </summary>
public class GetRecentRestaurantsQueryHandler : IRequestHandler<GetRecentRestaurantsQuery, List<RestaurantListDto>>
{
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IMapper _mapper;

    public GetRecentRestaurantsQueryHandler(IRepository<Restaurant> restaurantRepository, IMapper mapper)
    {
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
    }

    public async Task<List<RestaurantListDto>> Handle(GetRecentRestaurantsQuery request, CancellationToken cancellationToken)
    {
        var restaurants = await _restaurantRepository.GetAllAsync(cancellationToken);
        
        var recentRestaurants = restaurants
            .OrderByDescending(r => r.CreatedAt)
            .Take(request.Limit)
            .ToList();

        return _mapper.Map<List<RestaurantListDto>>(recentRestaurants);
    }
}

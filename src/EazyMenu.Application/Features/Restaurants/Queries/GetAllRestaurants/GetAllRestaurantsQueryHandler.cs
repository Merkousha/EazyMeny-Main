using AutoMapper;
using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Restaurant;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Restaurants.Queries.GetAllRestaurants;

/// <summary>
/// Handler برای دریافت تمام رستوران‌ها
/// </summary>
public class GetAllRestaurantsQueryHandler : IRequestHandler<GetAllRestaurantsQuery, List<RestaurantListDto>>
{
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IRepository<ApplicationUser> _userRepository;
    private readonly IMapper _mapper;

    public GetAllRestaurantsQueryHandler(
        IRepository<Restaurant> restaurantRepository,
        IRepository<ApplicationUser> userRepository,
        IMapper mapper)
    {
        _restaurantRepository = restaurantRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<RestaurantListDto>> Handle(GetAllRestaurantsQuery request, CancellationToken cancellationToken)
    {
        var restaurants = await _restaurantRepository.GetAllAsync(cancellationToken);

        var restaurantList = restaurants
            .OrderByDescending(r => r.CreatedAt)
            .ToList();

        var dtos = _mapper.Map<List<RestaurantListDto>>(restaurantList);

        // دریافت نام مالکان
        var ownerIds = restaurantList.Select(r => r.OwnerId).Distinct().ToList();
        var allUsers = await _userRepository.GetAllAsync(cancellationToken);
        var owners = allUsers.Where(u => ownerIds.Contains(u.Id)).ToList();
        var ownerDict = owners.ToDictionary(o => o.Id, o => o.FullName);

        foreach (var dto in dtos)
        {
            var restaurant = restaurantList.First(r => r.Id == dto.Id);
            if (ownerDict.TryGetValue(restaurant.OwnerId, out var ownerName))
            {
                dto.OwnerName = ownerName;
            }
        }

        return dtos;
    }
}

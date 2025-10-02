using AutoMapper;
using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Restaurant;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Restaurants.Queries.GetRestaurantsByOwner;

/// <summary>
/// Handler برای دریافت لیست رستوران‌های یک صاحب
/// </summary>
public class GetRestaurantsByOwnerQueryHandler : IRequestHandler<GetRestaurantsByOwnerQuery, List<RestaurantListDto>>
{
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IRepository<ApplicationUser> _userRepository;
    private readonly IMapper _mapper;

    public GetRestaurantsByOwnerQueryHandler(
        IRepository<Restaurant> restaurantRepository,
        IRepository<ApplicationUser> userRepository,
        IMapper mapper)
    {
        _restaurantRepository = restaurantRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<RestaurantListDto>> Handle(GetRestaurantsByOwnerQuery request, CancellationToken cancellationToken)
    {
        var allRestaurants = await _restaurantRepository.GetAllAsync(cancellationToken);
        var restaurants = allRestaurants.Where(r => r.OwnerId == request.OwnerId).ToList();

        var restaurantList = restaurants
            .OrderByDescending(r => r.CreatedAt)
            .ToList();

        var dtos = _mapper.Map<List<RestaurantListDto>>(restaurantList);

        // دریافت نام مالک
        var owner = await _userRepository.GetByIdAsync(request.OwnerId, cancellationToken);
        if (owner != null)
        {
            foreach (var dto in dtos)
            {
                dto.OwnerName = owner.FullName;
            }
        }

        return dtos;
    }
}

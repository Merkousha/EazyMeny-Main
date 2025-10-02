using AutoMapper;
using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Restaurant;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Restaurants.Queries.GetRestaurantById;

/// <summary>
/// Handler برای دریافت اطلاعات رستوران بر اساس Id
/// </summary>
public class GetRestaurantByIdQueryHandler : IRequestHandler<GetRestaurantByIdQuery, RestaurantDto?>
{
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IRepository<ApplicationUser> _userRepository;
    private readonly IMapper _mapper;

    public GetRestaurantByIdQueryHandler(
        IRepository<Restaurant> restaurantRepository,
        IRepository<ApplicationUser> userRepository,
        IMapper mapper)
    {
        _restaurantRepository = restaurantRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<RestaurantDto?> Handle(GetRestaurantByIdQuery request, CancellationToken cancellationToken)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(request.Id, cancellationToken);

        if (restaurant == null)
        {
            return null;
        }

        var dto = _mapper.Map<RestaurantDto>(restaurant);

        // دریافت نام مالک
        var owner = await _userRepository.GetByIdAsync(restaurant.OwnerId, cancellationToken);
        if (owner != null)
        {
            dto.OwnerName = owner.FullName;
        }

        return dto;
    }
}

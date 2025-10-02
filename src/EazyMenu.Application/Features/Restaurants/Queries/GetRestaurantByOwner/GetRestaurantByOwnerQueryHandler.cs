using AutoMapper;
using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Restaurants.Queries.GetRestaurantByOwner;

/// <summary>
/// Handler برای دریافت رستوران بر اساس صاحب رستوران
/// </summary>
public class GetRestaurantByOwnerQueryHandler : IRequestHandler<GetRestaurantByOwnerQuery, RestaurantBasicDto?>
{
    private readonly IRepository<Restaurant> _repository;
    private readonly IMapper _mapper;

    public GetRestaurantByOwnerQueryHandler(IRepository<Restaurant> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<RestaurantBasicDto?> Handle(GetRestaurantByOwnerQuery request, CancellationToken cancellationToken)
    {
        // دریافت رستوران بر اساس OwnerId
        var restaurants = await _repository.GetAllAsync(cancellationToken);
        var restaurant = restaurants
            .Where(r => r.OwnerId == request.OwnerId && r.IsActive)
            .FirstOrDefault();

        if (restaurant == null)
            return null;

        return _mapper.Map<RestaurantBasicDto>(restaurant);
    }
}

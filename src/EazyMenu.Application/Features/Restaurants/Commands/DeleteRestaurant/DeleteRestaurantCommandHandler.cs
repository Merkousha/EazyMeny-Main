using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Restaurants.Commands.DeleteRestaurant;

/// <summary>
/// Handler برای حذف نرم رستوران
/// </summary>
public class DeleteRestaurantCommandHandler : IRequestHandler<DeleteRestaurantCommand, bool>
{
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRestaurantCommandHandler(
        IRepository<Restaurant> restaurantRepository,
        IUnitOfWork unitOfWork)
    {
        _restaurantRepository = restaurantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteRestaurantCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(request.Id, cancellationToken);

        if (restaurant == null)
        {
            return false;
        }

        // Soft Delete - BaseEntity has IsDeleted property
        restaurant.IsDeleted = true;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

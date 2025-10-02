using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Restaurants.Commands.UpdateRestaurant;

/// <summary>
/// Handler برای ویرایش رستوران
/// </summary>
public class UpdateRestaurantCommandHandler : IRequestHandler<UpdateRestaurantCommand, bool>
{
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRestaurantCommandHandler(
        IRepository<Restaurant> restaurantRepository,
        IUnitOfWork unitOfWork)
    {
        _restaurantRepository = restaurantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateRestaurantCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(request.Id, cancellationToken);

        if (restaurant == null)
        {
            return false;
        }

        // به‌روزرسانی اطلاعات
        restaurant.Name = request.Name;
        restaurant.NameEn = request.NameEn;
        restaurant.Description = request.Description;
        restaurant.ManagerName = request.ManagerName;
        restaurant.Address = request.Address;
        restaurant.PhoneNumber = request.PhoneNumber;
        restaurant.Email = request.Email;
        restaurant.WorkingHours = request.WorkingHours;
        restaurant.IsActive = request.IsActive;
        restaurant.AcceptReservations = request.AcceptReservations;
        restaurant.AcceptOnlineOrders = request.AcceptOnlineOrders;
        restaurant.DeliveryFee = request.DeliveryFee;
        restaurant.MinimumOrderAmount = request.MinimumOrderAmount;

        // EF Core tracks changes automatically
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

using MediatR;

namespace EazyMenu.Application.Features.Restaurants.Commands.DeleteRestaurant;

/// <summary>
/// Command برای حذف (نرم) رستوران
/// </summary>
public class DeleteRestaurantCommand : IRequest<bool>
{
    public Guid Id { get; set; }

    public DeleteRestaurantCommand(Guid id)
    {
        Id = id;
    }
}

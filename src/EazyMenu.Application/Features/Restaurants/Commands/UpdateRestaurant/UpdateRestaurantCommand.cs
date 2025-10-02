using MediatR;

namespace EazyMenu.Application.Features.Restaurants.Commands.UpdateRestaurant;

/// <summary>
/// Command برای ویرایش رستوران
/// </summary>
public class UpdateRestaurantCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ManagerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? WorkingHours { get; set; }
    
    public bool IsActive { get; set; }
    public bool AcceptOnlineOrders { get; set; }
    public bool AcceptReservations { get; set; }
    
    public decimal DeliveryFee { get; set; }
    public decimal MinimumOrderAmount { get; set; }
}

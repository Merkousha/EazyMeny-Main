using MediatR;

namespace EazyMenu.Application.Features.Restaurants.Commands.CreateRestaurant;

/// <summary>
/// Command برای ایجاد رستوران جدید
/// </summary>
public class CreateRestaurantCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ManagerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? WorkingHours { get; set; }
    
    public bool IsActive { get; set; } = true;
    public bool AcceptOnlineOrders { get; set; } = true;
    public bool AcceptReservations { get; set; } = false;
    
    public decimal DeliveryFee { get; set; } = 0;
    public decimal MinimumOrderAmount { get; set; } = 0;
    
    // صاحب رستوران (از کاربر احراز شده)
    public Guid OwnerId { get; set; }
}

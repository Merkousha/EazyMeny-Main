using MediatR;

namespace EazyMenu.Application.Features.Reservations.Commands.CreateReservation;

/// <summary>
/// Command برای ایجاد رزرو میز جدید
/// </summary>
public class CreateReservationCommand : IRequest<Guid>
{
    public Guid RestaurantId { get; set; }
    
    // مشتری
    public Guid? CustomerId { get; set; } // اگر لاگین کرده باشه
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    
    // زمان رزرو
    public DateTime ReservationDate { get; set; }
    public TimeSpan ReservationTime { get; set; }
    public int GuestsCount { get; set; }
    
    // جزئیات
    public string? SpecialRequests { get; set; }
}

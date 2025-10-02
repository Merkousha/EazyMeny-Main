using MediatR;

namespace EazyMenu.Application.Features.Reservations.Commands.CancelReservation;

/// <summary>
/// Command برای لغو رزرو
/// </summary>
public class CancelReservationCommand : IRequest<bool>
{
    public Guid ReservationId { get; set; }
    public string? CancellationReason { get; set; }
}
using MediatR;

namespace EazyMenu.Application.Features.Reservations.Commands.ConfirmReservation;

/// <summary>
/// Command برای تایید رزرو توسط رستوران
/// </summary>
public class ConfirmReservationCommand : IRequest<bool>
{
    public Guid ReservationId { get; set; }
    public int? TableNumber { get; set; }
}
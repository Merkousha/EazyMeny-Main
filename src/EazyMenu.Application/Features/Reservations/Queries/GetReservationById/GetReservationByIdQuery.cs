using EazyMenu.Application.Common.Models.Reservation;
using MediatR;

namespace EazyMenu.Application.Features.Reservations.Queries.GetReservationById;

/// <summary>
/// Query برای دریافت جزئیات یک رزرو
/// </summary>
public class GetReservationByIdQuery : IRequest<ReservationDto?>
{
    public Guid Id { get; set; }
}

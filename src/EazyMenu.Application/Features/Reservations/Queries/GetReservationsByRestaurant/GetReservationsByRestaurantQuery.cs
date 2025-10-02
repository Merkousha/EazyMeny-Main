using EazyMenu.Application.Common.Models.Reservation;
using EazyMenu.Domain.Enums;
using MediatR;

namespace EazyMenu.Application.Features.Reservations.Queries.GetReservationsByRestaurant;

/// <summary>
/// Query برای دریافت رزروهای یک رستوران
/// </summary>
public class GetReservationsByRestaurantQuery : IRequest<List<ReservationListDto>>
{
    public Guid RestaurantId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public ReservationStatus? Status { get; set; }
}

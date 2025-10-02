using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Reservation;
using EazyMenu.Domain.Entities;
using EazyMenu.Domain.Enums;
using MediatR;

namespace EazyMenu.Application.Features.Reservations.Queries.GetReservationsByDate;

/// <summary>
/// Query برای دریافت رزروهای یک تاریخ خاص (برای نمایش کلندر)
/// </summary>
public class GetReservationsByDateQuery : IRequest<List<ReservationListDto>>
{
    public Guid RestaurantId { get; set; }
    public DateTime Date { get; set; }
    public ReservationStatus? Status { get; set; }
}

/// <summary>
/// Handler برای GetReservationsByDateQuery
/// </summary>
public class GetReservationsByDateQueryHandler : IRequestHandler<GetReservationsByDateQuery, List<ReservationListDto>>
{
    private readonly IRepository<Reservation> _reservationRepository;

    public GetReservationsByDateQueryHandler(IRepository<Reservation> reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<List<ReservationListDto>> Handle(GetReservationsByDateQuery request, CancellationToken cancellationToken)
    {
        var allReservations = await _reservationRepository.GetAllAsync(cancellationToken);

        var query = allReservations
            .Where(r => r.RestaurantId == request.RestaurantId)
            .Where(r => r.ReservationDate.Date == request.Date.Date);

        // فیلتر وضعیت
        if (request.Status.HasValue)
        {
            query = query.Where(r => r.Status == request.Status.Value);
        }

        var reservations = query
            .OrderBy(r => r.ReservationTime)
            .ToList();

        // Load Restaurant names
        var reservationsWithRestaurant = await _reservationRepository
            .FindWithIncludesAsync(
                r => reservations.Select(res => res.Id).Contains(r.Id),
                cancellationToken,
                r => r.Restaurant);

        return reservationsWithRestaurant.Select(r => new ReservationListDto
        {
            Id = r.Id,
            ReservationNumber = r.ReservationNumber,
            RestaurantName = r.Restaurant?.Name ?? "",
            CustomerName = r.CustomerName,
            CustomerPhone = r.CustomerPhone,
            ReservationDate = r.ReservationDate,
            ReservationTime = r.ReservationTime,
            GuestsCount = r.GuestsCount,
            TableNumber = r.TableNumber,
            Status = r.Status,
            CreatedAt = r.CreatedAt
        })
        .OrderBy(r => r.ReservationTime)
        .ToList();
    }
}

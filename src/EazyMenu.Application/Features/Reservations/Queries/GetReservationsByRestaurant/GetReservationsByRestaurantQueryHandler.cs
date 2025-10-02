using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Reservation;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Reservations.Queries.GetReservationsByRestaurant;

/// <summary>
/// Handler برای GetReservationsByRestaurantQuery
/// </summary>
public class GetReservationsByRestaurantQueryHandler : IRequestHandler<GetReservationsByRestaurantQuery, List<ReservationListDto>>
{
    private readonly IRepository<Reservation> _reservationRepository;

    public GetReservationsByRestaurantQueryHandler(IRepository<Reservation> reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<List<ReservationListDto>> Handle(GetReservationsByRestaurantQuery request, CancellationToken cancellationToken)
    {
        var allReservations = await _reservationRepository.GetAllAsync(cancellationToken);

        var query = allReservations
            .Where(r => r.RestaurantId == request.RestaurantId);

        // فیلتر تاریخ
        if (request.FromDate.HasValue)
        {
            query = query.Where(r => r.ReservationDate >= request.FromDate.Value.Date);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(r => r.ReservationDate <= request.ToDate.Value.Date);
        }

        // فیلتر وضعیت
        if (request.Status.HasValue)
        {
            query = query.Where(r => r.Status == request.Status.Value);
        }

        var reservations = query
            .OrderByDescending(r => r.ReservationDate)
            .ThenByDescending(r => r.ReservationTime)
            .ToList();

        // Map to DTO with Restaurant name (need to load Restaurant)
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
        .OrderByDescending(r => r.ReservationDate)
        .ThenByDescending(r => r.ReservationTime)
        .ToList();
    }
}

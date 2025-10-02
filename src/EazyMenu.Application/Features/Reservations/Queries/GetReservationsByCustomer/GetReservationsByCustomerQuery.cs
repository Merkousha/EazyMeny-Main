using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Reservation;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Reservations.Queries.GetReservationsByCustomer;

/// <summary>
/// Query برای دریافت رزروهای یک مشتری
/// </summary>
public class GetReservationsByCustomerQuery : IRequest<List<ReservationListDto>>
{
    public Guid? CustomerId { get; set; }
    public string? CustomerPhone { get; set; }
}

/// <summary>
/// Handler برای GetReservationsByCustomerQuery
/// </summary>
public class GetReservationsByCustomerQueryHandler : IRequestHandler<GetReservationsByCustomerQuery, List<ReservationListDto>>
{
    private readonly IRepository<Reservation> _reservationRepository;

    public GetReservationsByCustomerQueryHandler(IRepository<Reservation> reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<List<ReservationListDto>> Handle(GetReservationsByCustomerQuery request, CancellationToken cancellationToken)
    {
        var allReservations = await _reservationRepository.GetAllAsync(cancellationToken);

        var query = allReservations.AsEnumerable();

        // فیلتر بر اساس CustomerId یا CustomerPhone
        if (request.CustomerId.HasValue)
        {
            query = query.Where(r => r.CustomerId == request.CustomerId.Value);
        }
        else if (!string.IsNullOrEmpty(request.CustomerPhone))
        {
            query = query.Where(r => r.CustomerPhone == request.CustomerPhone);
        }
        else
        {
            return new List<ReservationListDto>();
        }

        var reservations = query
            .OrderByDescending(r => r.ReservationDate)
            .ThenByDescending(r => r.ReservationTime)
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
        .OrderByDescending(r => r.ReservationDate)
        .ThenByDescending(r => r.ReservationTime)
        .ToList();
    }
}

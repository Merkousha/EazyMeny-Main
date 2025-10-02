using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Reservation;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Reservations.Queries.GetReservationById;

/// <summary>
/// Handler برای GetReservationByIdQuery
/// </summary>
public class GetReservationByIdQueryHandler : IRequestHandler<GetReservationByIdQuery, ReservationDto?>
{
    private readonly IRepository<Reservation> _reservationRepository;

    public GetReservationByIdQueryHandler(IRepository<Reservation> reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<ReservationDto?> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository
            .GetByIdWithIncludesAsync(
                request.Id,
                cancellationToken,
                r => r.Restaurant,
                r => r.Customer);

        if (reservation == null)
            return null;

        return new ReservationDto
        {
            Id = reservation.Id,
            ReservationNumber = reservation.ReservationNumber,
            
            // Restaurant
            RestaurantId = reservation.RestaurantId,
            RestaurantName = reservation.Restaurant?.Name ?? "",
            RestaurantPhone = reservation.Restaurant?.PhoneNumber ?? "",
            RestaurantAddress = reservation.Restaurant?.Address ?? "",
            
            // Customer
            CustomerId = reservation.CustomerId,
            CustomerName = reservation.CustomerName,
            CustomerPhone = reservation.CustomerPhone,
            CustomerEmail = reservation.CustomerEmail,
            
            // Reservation
            ReservationDate = reservation.ReservationDate,
            ReservationTime = reservation.ReservationTime,
            GuestsCount = reservation.GuestsCount,
            Status = reservation.Status,
            
            // Details
            SpecialRequests = reservation.SpecialRequests,
            TableNumber = reservation.TableNumber,
            IsNoShow = reservation.IsNoShow,
            
            // Timestamps
            CheckedInAt = reservation.CheckedInAt,
            CancelledAt = reservation.CancelledAt,
            CancellationReason = reservation.CancellationReason,
            ReminderSent = reservation.ReminderSent,
            ReminderSentAt = reservation.ReminderSentAt,
            CreatedAt = reservation.CreatedAt,
            UpdatedAt = reservation.UpdatedAt
        };
    }
}

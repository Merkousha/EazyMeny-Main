using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using EazyMenu.Domain.Enums;
using MediatR;

namespace EazyMenu.Application.Features.Reservations.Commands.CancelReservation;

/// <summary>
/// Handler برای CancelReservationCommand
/// </summary>
public class CancelReservationCommandHandler : IRequestHandler<CancelReservationCommand, bool>
{
    private readonly IRepository<Reservation> _reservationRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISmsService _smsService;

    public CancelReservationCommandHandler(
        IRepository<Reservation> reservationRepository,
        IRepository<Restaurant> restaurantRepository,
        IUnitOfWork unitOfWork,
        ISmsService smsService)
    {
        _reservationRepository = reservationRepository;
        _restaurantRepository = restaurantRepository;
        _unitOfWork = unitOfWork;
        _smsService = smsService;
    }

    public async Task<bool> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        // پیدا کردن رزرو
        var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId, cancellationToken);
        if (reservation == null)
        {
            throw new InvalidOperationException("رزرو یافت نشد");
        }

        // بررسی اینکه قبلاً لغو نشده باشد
        if (reservation.Status == ReservationStatus.Cancelled)
        {
            throw new InvalidOperationException("این رزرو قبلاً لغو شده است");
        }

        // بررسی اینکه رزرو در گذشته نباشد
        var reservationDateTime = reservation.ReservationDate.Add(reservation.ReservationTime);
        if (reservationDateTime < DateTime.UtcNow)
        {
            throw new InvalidOperationException("نمی‌توان رزرو گذشته را لغو کرد");
        }

        // لغو رزرو
        reservation.Status = ReservationStatus.Cancelled;
        reservation.CancelledAt = DateTime.UtcNow;
        reservation.CancellationReason = request.CancellationReason ?? "لغو شده توسط مشتری";

        await _reservationRepository.UpdateAsync(reservation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // ارسال SMS لغو رزرو
        try
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(reservation.RestaurantId, cancellationToken);
            if (restaurant != null)
            {
                var smsMessage = $"رزرو شما در رستوران {restaurant.Name}\n" +
                               $"شماره رزرو: {reservation.ReservationNumber}\n" +
                               $"لغو شد.\n" +
                               $"امیدواریم در فرصت دیگری خدمتتان باشیم.";

                await _smsService.SendAsync(reservation.CustomerPhone, smsMessage, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            // لاگ ارور اما عملیات لغو موفق است
            Console.WriteLine($"⚠️ SMS send failed: {ex.Message}");
        }

        return true;
    }
}
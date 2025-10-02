using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using EazyMenu.Domain.Enums;
using MediatR;

namespace EazyMenu.Application.Features.Reservations.Commands.ConfirmReservation;

/// <summary>
/// Handler برای ConfirmReservationCommand
/// </summary>
public class ConfirmReservationCommandHandler : IRequestHandler<ConfirmReservationCommand, bool>
{
    private readonly IRepository<Reservation> _reservationRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISmsService _smsService;

    public ConfirmReservationCommandHandler(
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

    public async Task<bool> Handle(ConfirmReservationCommand request, CancellationToken cancellationToken)
    {
        // پیدا کردن رزرو
        var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId, cancellationToken);
        if (reservation == null)
        {
            throw new InvalidOperationException("رزرو یافت نشد");
        }

        // بررسی وضعیت
        if (reservation.Status != ReservationStatus.Pending)
        {
            throw new InvalidOperationException("فقط رزروهای در انتظار قابل تایید هستند");
        }

        // تایید رزرو
        reservation.Status = ReservationStatus.Confirmed;
        if (request.TableNumber.HasValue)
        {
            reservation.TableNumber = request.TableNumber.Value;
        }

        await _reservationRepository.UpdateAsync(reservation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // ارسال SMS تایید رزرو
        try
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(reservation.RestaurantId, cancellationToken);
            if (restaurant != null)
            {
                var reservationDateTime = reservation.ReservationDate.Add(reservation.ReservationTime);
                var persianDate = reservationDateTime.ToString("yyyy/MM/dd");
                var persianTime = reservationDateTime.ToString("HH:mm");
                
                var tableInfo = reservation.TableNumber.HasValue 
                    ? $"\nمیز شماره: {reservation.TableNumber}" 
                    : "";
                
                var smsMessage = $"رزرو شما در رستوران {restaurant.Name} تایید شد.\n" +
                               $"تاریخ: {persianDate}\n" +
                               $"ساعت: {persianTime}" +
                               tableInfo +
                               $"\nمنتظر حضور شما هستیم!";

                await _smsService.SendAsync(reservation.CustomerPhone, smsMessage, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            // لاگ ارور اما عملیات تایید موفق است
            Console.WriteLine($"⚠️ SMS send failed: {ex.Message}");
        }

        return true;
    }
}
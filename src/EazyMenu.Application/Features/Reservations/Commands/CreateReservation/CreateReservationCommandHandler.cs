using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using EazyMenu.Domain.Enums;
using MediatR;

namespace EazyMenu.Application.Features.Reservations.Commands.CreateReservation;

/// <summary>
/// Handler برای CreateReservationCommand
/// </summary>
public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, Guid>
{
    private readonly IRepository<Reservation> _reservationRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISmsService _smsService;

    public CreateReservationCommandHandler(
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

    public async Task<Guid> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        // بررسی وجود رستوران
        var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId, cancellationToken);
        if (restaurant == null)
        {
            throw new InvalidOperationException("رستوران یافت نشد");
        }

        // بررسی اینکه رستوران رزرو میز را فعال کرده باشد
        if (!restaurant.AcceptReservations)
        {
            throw new InvalidOperationException("این رستوران امکان رزرو میز را ندارد");
        }

        // تولید شماره رزرو یکتا
        var reservationNumber = await GenerateReservationNumberAsync(cancellationToken);

        // ایجاد رزرو جدید
        var reservation = new Reservation
        {
            ReservationNumber = reservationNumber,
            RestaurantId = request.RestaurantId,
            CustomerId = request.CustomerId,
            CustomerName = request.CustomerName,
            CustomerPhone = request.CustomerPhone,
            CustomerEmail = request.CustomerEmail,
            ReservationDate = request.ReservationDate.Date,
            ReservationTime = request.ReservationTime,
            GuestsCount = request.GuestsCount,
            SpecialRequests = request.SpecialRequests,
            Status = ReservationStatus.Pending
        };

        await _reservationRepository.AddAsync(reservation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // ارسال SMS تایید رزرو به مشتری
        try
        {
            var reservationDateTime = reservation.ReservationDate.Add(reservation.ReservationTime);
            var persianDate = reservationDateTime.ToString("yyyy/MM/dd");
            var persianTime = reservationDateTime.ToString("HH:mm");
            
            var smsMessage = $"رزرو شما در رستوران {restaurant.Name}\n" +
                           $"تاریخ: {persianDate}\n" +
                           $"ساعت: {persianTime}\n" +
                           $"تعداد نفرات: {reservation.GuestsCount}\n" +
                           $"شماره رزرو: {reservation.ReservationNumber}\n" +
                           $"منتظر حضور شما هستیم!";

            await _smsService.SendAsync(request.CustomerPhone, smsMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            // لاگ ارور اما عملیات رزرو موفق است
            // TODO: Add logging
            Console.WriteLine($"⚠️ SMS send failed: {ex.Message}");
        }

        return reservation.Id;
    }

    /// <summary>
    /// تولید شماره رزرو یکتا با فرمت RES-YYYYMMDD-XXXXX
    /// </summary>
    private async Task<string> GenerateReservationNumberAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow;
        var datePrefix = today.ToString("yyyyMMdd");
        var prefix = $"RES-{datePrefix}-";

        // پیدا کردن آخرین شماره رزرو امروز
        var allReservations = await _reservationRepository.GetAllAsync(cancellationToken);
        var todayReservations = allReservations
            .Where(r => r.ReservationNumber.StartsWith(prefix))
            .ToList();

        var lastNumber = 0;
        if (todayReservations.Any())
        {
            var lastReservation = todayReservations
                .OrderByDescending(r => r.ReservationNumber)
                .First();
            
            var numberPart = lastReservation.ReservationNumber.Split('-').Last();
            int.TryParse(numberPart, out lastNumber);
        }

        var newNumber = (lastNumber + 1).ToString("D5");
        return $"{prefix}{newNumber}";
    }
}

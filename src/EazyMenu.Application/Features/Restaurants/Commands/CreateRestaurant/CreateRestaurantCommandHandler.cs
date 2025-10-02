using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Restaurants.Commands.CreateRestaurant;

/// <summary>
/// Handler برای ایجاد رستوران جدید
/// </summary>
public class CreateRestaurantCommandHandler : IRequestHandler<CreateRestaurantCommand, Guid>
{
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQRCodeService _qrCodeService;

    public CreateRestaurantCommandHandler(
        IRepository<Restaurant> restaurantRepository,
        IUnitOfWork unitOfWork,
        IQRCodeService qrCodeService)
    {
        _restaurantRepository = restaurantRepository;
        _unitOfWork = unitOfWork;
        _qrCodeService = qrCodeService;
    }

    public async Task<Guid> Handle(CreateRestaurantCommand request, CancellationToken cancellationToken)
    {
        // تولید Slug از نام رستوران
        var slug = GenerateSlug(request.Name);

        // بررسی تکراری بودن Slug
        var allRestaurants = await _restaurantRepository.GetAllAsync(cancellationToken);
        var existingRestaurant = allRestaurants.FirstOrDefault(r => r.Slug == slug);

        if (existingRestaurant != null)
        {
            // اگر Slug تکراری بود، یک شماره اضافه می‌کنیم
            slug = $"{slug}-{Guid.NewGuid().ToString()[..8]}";
        }

        // ایجاد رستوران جدید
        var restaurant = new Restaurant
        {
            Name = request.Name,
            NameEn = request.NameEn,
            Slug = slug,
            Description = request.Description,
            ManagerName = request.ManagerName,
            Address = request.Address,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            WorkingHours = request.WorkingHours,
            IsActive = request.IsActive,
            AcceptReservations = request.AcceptReservations,
            AcceptOnlineOrders = request.AcceptOnlineOrders,
            DeliveryFee = request.DeliveryFee,
            MinimumOrderAmount = request.MinimumOrderAmount,
            OwnerId = request.OwnerId
        };

        await _restaurantRepository.AddAsync(restaurant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // تولید QR Code برای رستوران (آدرس منوی دیجیتال)
        try
        {
            var menuUrl = $"https://eazymenu.ir/menu/{restaurant.Slug}";
            var qrCodePath = await _qrCodeService.SaveQRCodeAsync(
                restaurant.Id.ToString(),
                menuUrl,
                300,
                cancellationToken);

            restaurant.QRCodeUrl = qrCodePath;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
            // در صورت خطا در تولید QR Code، ثبت رستوران ادامه می‌یابد
            // می‌توان بعداً QR Code را دوباره تولید کرد
        }

        return restaurant.Id;
    }

    /// <summary>
    /// تولید Slug از نام رستوران
    /// </summary>
    private string GenerateSlug(string name)
    {
        // حذف فاصله‌های اضافی
        var slug = name.Trim().ToLowerInvariant();

        // جایگزینی فاصله‌ها با dash
        slug = slug.Replace(" ", "-");

        // حذف کاراکترهای غیرمجاز
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\u0600-\u06FF\-]", "");

        // حذف dash های متوالی
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\-+", "-");

        // حذف dash از ابتدا و انتها
        slug = slug.Trim('-');

        return slug;
    }
}

using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Website.Commands.UpdateCustomization;

/// <summary>
/// Handler به‌روزرسانی تنظیمات سفارشی‌سازی
/// </summary>
public class UpdateCustomizationCommandHandler : IRequestHandler<UpdateCustomizationCommand, bool>
{
    private readonly IRepository<WebsiteCustomization> _customizationRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomizationCommandHandler(
        IRepository<WebsiteCustomization> customizationRepository,
        IRepository<Restaurant> restaurantRepository,
        IUnitOfWork unitOfWork)
    {
        _customizationRepository = customizationRepository;
        _restaurantRepository = restaurantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateCustomizationCommand request, CancellationToken cancellationToken)
    {
        // بررسی وجود رستوران
        var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId, cancellationToken);
        if (restaurant == null)
            throw new Exception("رستوران یافت نشد");

        // دریافت تنظیمات فعلی
        var customizations = await _customizationRepository.GetAllAsync(cancellationToken);
        var customization = customizations.FirstOrDefault(c => c.RestaurantId == request.RestaurantId);

        if (customization == null)
        {
            // ایجاد جدید
            customization = new WebsiteCustomization
            {
                RestaurantId = request.RestaurantId
            };
            await _customizationRepository.AddAsync(customization, cancellationToken);
        }

        // به‌روزرسانی تنظیمات
        customization.PrimaryColor = request.PrimaryColor;
        customization.SecondaryColor = request.SecondaryColor;
        customization.TextColor = request.TextColor;
        customization.BackgroundColor = request.BackgroundColor;
        customization.FontFamily = request.FontFamily;
        customization.FontSize = request.FontSize;
        customization.CustomLogoUrl = request.CustomLogoUrl;
        customization.FaviconUrl = request.FaviconUrl;
        customization.SeoTitle = request.SeoTitle;
        customization.SeoDescription = request.SeoDescription;
        customization.SeoKeywords = request.SeoKeywords;
        customization.SocialImageUrl = request.SocialImageUrl;
        customization.GoogleAnalyticsId = request.GoogleAnalyticsId;
        customization.CustomCss = request.CustomCss;
        customization.CustomJs = request.CustomJs;

        await _customizationRepository.UpdateAsync(customization, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

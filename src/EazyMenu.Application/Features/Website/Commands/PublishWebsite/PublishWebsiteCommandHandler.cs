using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Website.Commands.PublishWebsite;

/// <summary>
/// Handler انتشار/عدم انتشار وب‌سایت
/// </summary>
public class PublishWebsiteCommandHandler : IRequestHandler<PublishWebsiteCommand, bool>
{
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IRepository<WebsiteContent> _contentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PublishWebsiteCommandHandler(
        IRepository<Restaurant> restaurantRepository,
        IRepository<WebsiteContent> contentRepository,
        IUnitOfWork unitOfWork)
    {
        _restaurantRepository = restaurantRepository;
        _contentRepository = contentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(PublishWebsiteCommand request, CancellationToken cancellationToken)
    {
        // بررسی وجود رستوران
        var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId, cancellationToken);
        if (restaurant == null)
            throw new Exception("رستوران یافت نشد");

        if (request.IsPublished)
        {
            // بررسی وجود محتوا قبل از انتشار
            var allContents = await _contentRepository.GetAllAsync(cancellationToken);
            var contents = allContents.Where(c => c.RestaurantId == request.RestaurantId).ToList();

            if (!contents.Any())
                throw new Exception("ابتدا یک قالب انتخاب کنید");

            // بررسی وجود بخش‌های الزامی
            var hasHero = contents.Any(c => c.SectionType == Domain.Enums.SectionType.Hero && c.IsVisible);
            var hasContact = contents.Any(c => c.SectionType == Domain.Enums.SectionType.Contact && c.IsVisible);

            if (!hasHero || !hasContact)
                throw new Exception("بخش‌های سربرگ و تماس با ما الزامی هستند");

            restaurant.IsWebsitePublished = true;
            restaurant.WebsitePublishedAt = DateTime.UtcNow;
        }
        else
        {
            restaurant.IsWebsitePublished = false;
            restaurant.WebsitePublishedAt = null;
        }

        await _restaurantRepository.UpdateAsync(restaurant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

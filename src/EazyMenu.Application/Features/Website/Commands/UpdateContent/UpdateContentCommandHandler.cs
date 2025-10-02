using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Website.Commands.UpdateContent;

/// <summary>
/// Handler به‌روزرسانی محتوا
/// </summary>
public class UpdateContentCommandHandler : IRequestHandler<UpdateContentCommand, bool>
{
    private readonly IRepository<WebsiteContent> _contentRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateContentCommandHandler(
        IRepository<WebsiteContent> contentRepository,
        IRepository<Restaurant> restaurantRepository,
        IUnitOfWork unitOfWork)
    {
        _contentRepository = contentRepository;
        _restaurantRepository = restaurantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateContentCommand request, CancellationToken cancellationToken)
    {
        // بررسی وجود رستوران
        var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId, cancellationToken);
        if (restaurant == null)
            throw new Exception("رستوران یافت نشد");

        // دریافت محتوای فعلی
        var allContents = await _contentRepository.GetAllAsync(cancellationToken);
        var content = allContents.FirstOrDefault(c => 
            c.RestaurantId == request.RestaurantId && 
            c.SectionType == request.SectionType);

        if (content == null)
            throw new Exception("محتوای این بخش یافت نشد. ابتدا یک قالب انتخاب کنید");

        // به‌روزرسانی محتوا
        content.Content = request.Content;
        content.UseDefaultContent = request.UseDefaultContent;
        content.IsVisible = request.IsVisible;

        await _contentRepository.UpdateAsync(content, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

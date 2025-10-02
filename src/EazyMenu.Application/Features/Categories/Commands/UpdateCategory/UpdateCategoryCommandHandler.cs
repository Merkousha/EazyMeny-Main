using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Categories.Commands.UpdateCategory;

/// <summary>
/// Handler برای ویرایش دسته‌بندی
/// </summary>
public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, bool>
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(
        IRepository<Category> categoryRepository,
        IRepository<Restaurant> restaurantRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _restaurantRepository = restaurantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        // بررسی وجود دسته‌بندی
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            throw new ArgumentException("دسته‌بندی مورد نظر یافت نشد");
        }

        // بررسی وجود رستوران
        var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId, cancellationToken);
        if (restaurant == null)
        {
            throw new ArgumentException("رستوران مورد نظر یافت نشد");
        }

        // به‌روزرسانی اطلاعات
        category.RestaurantId = request.RestaurantId;
        category.Name = request.Name;
        category.NameEn = request.NameEn;
        category.Description = request.Description;
        category.IconUrl = request.IconUrl;
        category.DisplayOrder = request.DisplayOrder;
        category.IsActive = request.IsActive;

        await _categoryRepository.UpdateAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

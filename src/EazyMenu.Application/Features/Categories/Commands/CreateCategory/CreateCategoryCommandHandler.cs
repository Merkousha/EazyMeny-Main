using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Categories.Commands.CreateCategory;

/// <summary>
/// Handler برای ایجاد دسته‌بندی جدید
/// </summary>
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(
        IRepository<Category> categoryRepository,
        IRepository<Restaurant> restaurantRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _restaurantRepository = restaurantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // بررسی وجود رستوران
        var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId, cancellationToken);
        if (restaurant == null)
        {
            throw new ArgumentException("رستوران مورد نظر یافت نشد");
        }

        // ایجاد دسته‌بندی جدید
        var category = new Category
        {
            RestaurantId = request.RestaurantId,
            Name = request.Name,
            NameEn = request.NameEn,
            Description = request.Description,
            IconUrl = request.IconUrl,
            DisplayOrder = request.DisplayOrder,
            IsActive = request.IsActive
        };

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}

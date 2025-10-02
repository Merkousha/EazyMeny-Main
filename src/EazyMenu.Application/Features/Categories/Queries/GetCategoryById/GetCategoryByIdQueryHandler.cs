using AutoMapper;
using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Category;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Categories.Queries.GetCategoryById;

/// <summary>
/// Handler برای دریافت اطلاعات یک دسته‌بندی
/// </summary>
public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IMapper _mapper;

    public GetCategoryByIdQueryHandler(
        IRepository<Category> categoryRepository,
        IRepository<Restaurant> restaurantRepository,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
    }

    public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category == null)
            return null;

        var restaurant = await _restaurantRepository.GetByIdAsync(category.RestaurantId, cancellationToken);

        var categoryDto = new CategoryDto
        {
            Id = category.Id,
            RestaurantId = category.RestaurantId,
            RestaurantName = restaurant?.Name ?? "نامشخص",
            Name = category.Name,
            NameEn = category.NameEn,
            Description = category.Description,
            IconUrl = category.IconUrl,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };

        return categoryDto;
    }
}

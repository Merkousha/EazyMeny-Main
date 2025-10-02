using AutoMapper;
using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Category;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Categories.Queries.GetAllCategories;

/// <summary>
/// Handler برای دریافت لیست تمام دسته‌بندی‌ها
/// </summary>
public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryListDto>>
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IMapper _mapper;

    public GetAllCategoriesQueryHandler(
        IRepository<Category> categoryRepository,
        IRepository<Restaurant> restaurantRepository,
        IRepository<Product> productRepository,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _restaurantRepository = restaurantRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<List<CategoryListDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        var restaurants = await _restaurantRepository.GetAllAsync(cancellationToken);
        var products = await _productRepository.GetAllAsync(cancellationToken);

        var categoryDtos = categories
            .OrderBy(c => c.RestaurantId)
            .ThenBy(c => c.DisplayOrder)
            .Select(c => new CategoryListDto
            {
                Id = c.Id,
                RestaurantId = c.RestaurantId,
                RestaurantName = restaurants.FirstOrDefault(r => r.Id == c.RestaurantId)?.Name ?? "نامشخص",
                Name = c.Name,
                NameEn = c.NameEn,
                DisplayOrder = c.DisplayOrder,
                IsActive = c.IsActive,
                ProductCount = products.Count(p => p.CategoryId == c.Id),
                CreatedAt = c.CreatedAt
            })
            .ToList();

        return categoryDtos;
    }
}

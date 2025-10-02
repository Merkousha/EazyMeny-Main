using AutoMapper;
using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Category;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Categories.Queries.GetCategoriesByRestaurant;

/// <summary>
/// Handler برای دریافت دسته‌بندی‌های یک رستوران
/// </summary>
public class GetCategoriesByRestaurantQueryHandler : IRequestHandler<GetCategoriesByRestaurantQuery, List<CategoryListDto>>
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IMapper _mapper;

    public GetCategoriesByRestaurantQueryHandler(
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

    public async Task<List<CategoryListDto>> Handle(GetCategoriesByRestaurantQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId, cancellationToken);
        var products = await _productRepository.GetAllAsync(cancellationToken);

        var restaurantCategories = categories
            .Where(c => c.RestaurantId == request.RestaurantId)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CategoryListDto
            {
                Id = c.Id,
                RestaurantId = c.RestaurantId,
                RestaurantName = restaurant?.Name ?? "نامشخص",
                Name = c.Name,
                NameEn = c.NameEn,
                DisplayOrder = c.DisplayOrder,
                IsActive = c.IsActive,
                ProductCount = products.Count(p => p.CategoryId == c.Id),
                CreatedAt = c.CreatedAt
            })
            .ToList();

        return restaurantCategories;
    }
}

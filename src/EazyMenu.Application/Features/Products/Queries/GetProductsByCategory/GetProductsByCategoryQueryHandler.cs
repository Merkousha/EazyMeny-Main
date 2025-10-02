using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Product;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Products.Queries.GetProductsByCategory;

/// <summary>
/// Handler برای دریافت محصولات یک دسته‌بندی
/// </summary>
public class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, List<ProductListDto>>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IRepository<Category> _categoryRepository;

    public GetProductsByCategoryQueryHandler(
        IRepository<Product> productRepository,
        IRepository<Restaurant> restaurantRepository,
        IRepository<Category> categoryRepository)
    {
        _productRepository = productRepository;
        _restaurantRepository = restaurantRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<List<ProductListDto>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        var restaurant = category != null ? await _restaurantRepository.GetByIdAsync(category.RestaurantId, cancellationToken) : null;

        var categoryProducts = products
            .Where(p => p.CategoryId == request.CategoryId)
            .OrderBy(p => p.DisplayOrder)
            .Select(p => new ProductListDto
            {
                Id = p.Id,
                RestaurantId = p.RestaurantId,
                RestaurantName = restaurant?.Name ?? "نامشخص",
                CategoryId = p.CategoryId,
                CategoryName = category?.Name ?? "نامشخص",
                Name = p.Name,
                NameEn = p.NameEn,
                Price = p.Price,
                DiscountedPrice = p.DiscountedPrice,
                Image1Url = p.Image1Url,
                DisplayOrder = p.DisplayOrder,
                IsActive = p.IsActive,
                IsAvailable = p.IsAvailable,
                IsNew = p.IsNew,
                IsPopular = p.IsPopular,
                StockQuantity = p.StockQuantity,
                CreatedAt = p.CreatedAt
            })
            .ToList();

        return categoryProducts;
    }
}

using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Product;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Products.Queries.GetProductsByRestaurant;

/// <summary>
/// Handler برای دریافت محصولات یک رستوران
/// </summary>
public class GetProductsByRestaurantQueryHandler : IRequestHandler<GetProductsByRestaurantQuery, List<ProductListDto>>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IRepository<Category> _categoryRepository;

    public GetProductsByRestaurantQueryHandler(
        IRepository<Product> productRepository,
        IRepository<Restaurant> restaurantRepository,
        IRepository<Category> categoryRepository)
    {
        _productRepository = productRepository;
        _restaurantRepository = restaurantRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<List<ProductListDto>> Handle(GetProductsByRestaurantQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId, cancellationToken);
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);

        var restaurantProducts = products
            .Where(p => p.RestaurantId == request.RestaurantId)
            .OrderBy(p => p.CategoryId)
            .ThenBy(p => p.DisplayOrder)
            .Select(p => new ProductListDto
            {
                Id = p.Id,
                RestaurantId = p.RestaurantId,
                RestaurantName = restaurant?.Name ?? "نامشخص",
                CategoryId = p.CategoryId,
                CategoryName = categories.FirstOrDefault(c => c.Id == p.CategoryId)?.Name ?? "نامشخص",
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

        return restaurantProducts;
    }
}

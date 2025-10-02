using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Product;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Products.Queries.GetProductById;

/// <summary>
/// Handler برای دریافت اطلاعات یک محصول
/// </summary>
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IRepository<Category> _categoryRepository;

    public GetProductByIdQueryHandler(
        IRepository<Product> productRepository,
        IRepository<Restaurant> restaurantRepository,
        IRepository<Category> categoryRepository)
    {
        _productRepository = productRepository;
        _restaurantRepository = restaurantRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
            return null;

        var restaurant = await _restaurantRepository.GetByIdAsync(product.RestaurantId, cancellationToken);
        var category = await _categoryRepository.GetByIdAsync(product.CategoryId, cancellationToken);

        var productDto = new ProductDto
        {
            Id = product.Id,
            RestaurantId = product.RestaurantId,
            RestaurantName = restaurant?.Name ?? "نامشخص",
            CategoryId = product.CategoryId,
            CategoryName = category?.Name ?? "نامشخص",
            Name = product.Name,
            NameEn = product.NameEn,
            Description = product.Description,
            Price = product.Price,
            DiscountedPrice = product.DiscountedPrice,
            Image1Url = product.Image1Url,
            Image2Url = product.Image2Url,
            Image3Url = product.Image3Url,
            DisplayOrder = product.DisplayOrder,
            IsActive = product.IsActive,
            IsAvailable = product.IsAvailable,
            IsNew = product.IsNew,
            IsPopular = product.IsPopular,
            IsSpicy = product.IsSpicy,
            IsVegetarian = product.IsVegetarian,
            StockQuantity = product.StockQuantity,
            PreparationTime = product.PreparationTime,
            Options = product.Options,
            NutritionalInfo = product.NutritionalInfo,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };

        return productDto;
    }
}

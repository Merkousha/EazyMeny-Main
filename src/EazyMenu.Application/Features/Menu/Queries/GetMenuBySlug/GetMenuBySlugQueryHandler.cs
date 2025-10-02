using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Menu;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Menu.Queries.GetMenuBySlug;

/// <summary>
/// Handler برای دریافت منوی عمومی رستوران
/// </summary>
public class GetMenuBySlugQueryHandler : IRequestHandler<GetMenuBySlugQuery, RestaurantMenuDto?>
{
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Product> _productRepository;

    public GetMenuBySlugQueryHandler(
        IRepository<Restaurant> restaurantRepository,
        IRepository<Category> categoryRepository,
        IRepository<Product> productRepository)
    {
        _restaurantRepository = restaurantRepository;
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
    }

    public async Task<RestaurantMenuDto?> Handle(GetMenuBySlugQuery request, CancellationToken cancellationToken)
    {
        // یافتن رستوران با Slug
        var restaurants = await _restaurantRepository.FindAsync(
            r => r.Slug == request.Slug && r.IsActive, 
            cancellationToken);
        
        var restaurant = restaurants.FirstOrDefault();
        if (restaurant == null)
            return null;

        // دریافت دسته‌بندی‌ها
        var categories = await _categoryRepository.FindAsync(
            c => c.RestaurantId == restaurant.Id,
            cancellationToken);

        var categoryList = categories
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToList();

        // دریافت محصولات
        var products = await _productRepository.FindAsync(
            p => p.RestaurantId == restaurant.Id && p.IsActive,
            cancellationToken);

        var productList = products.ToList();

        // ساخت DTO
        var menuDto = new RestaurantMenuDto
        {
            Id = restaurant.Id,
            Name = restaurant.Name,
            Slug = restaurant.Slug,
            Description = restaurant.Description,
            LogoUrl = restaurant.LogoUrl,
            CoverImageUrl = restaurant.CoverImageUrl,
            PhoneNumber = restaurant.PhoneNumber,
            Address = restaurant.Address,
            WorkingHours = restaurant.WorkingHours,
            IsActive = restaurant.IsActive,
            Categories = categoryList.Select(category => new CategoryWithProductsDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IconUrl = category.IconUrl,
                DisplayOrder = category.DisplayOrder,
                Products = productList
                    .Where(p => p.CategoryId == category.Id)
                    .OrderBy(p => p.DisplayOrder)
                    .ThenBy(p => p.Name)
                    .Select(product => new ProductMenuDto
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        DiscountedPrice = product.DiscountedPrice,
                        Image1Url = product.Image1Url,
                        IsAvailable = product.StockQuantity == null || product.StockQuantity > 0,
                        IsNew = product.IsNew,
                        IsPopular = product.IsPopular,
                        IsSpicy = product.IsSpicy,
                        IsVegetarian = product.IsVegetarian,
                        PreparationTime = product.PreparationTime
                    })
                    .ToList()
            })
            .Where(c => c.Products.Any()) // فقط دسته‌هایی که محصول دارند
            .ToList()
        };

        return menuDto;
    }
}

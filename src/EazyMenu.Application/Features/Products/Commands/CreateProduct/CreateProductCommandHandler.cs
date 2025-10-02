using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Products.Commands.CreateProduct;

/// <summary>
/// Handler برای ایجاد محصول جدید
/// </summary>
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IRepository<Product> productRepository,
        IRepository<Restaurant> restaurantRepository,
        IRepository<Category> categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _restaurantRepository = restaurantRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // بررسی وجود رستوران
        var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId, cancellationToken);
        if (restaurant == null)
            throw new Exception($"رستوران با شناسه {request.RestaurantId} یافت نشد.");

        // بررسی وجود دسته‌بندی
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null)
            throw new Exception($"دسته‌بندی با شناسه {request.CategoryId} یافت نشد.");

        // بررسی تعلق دسته‌بندی به رستوران
        if (category.RestaurantId != request.RestaurantId)
            throw new Exception("دسته‌بندی متعلق به این رستوران نیست.");

        var product = new Product
        {
            RestaurantId = request.RestaurantId,
            CategoryId = request.CategoryId,
            Name = request.Name,
            NameEn = request.NameEn,
            Description = request.Description,
            Price = request.Price,
            DiscountedPrice = request.DiscountedPrice,
            Image1Url = request.Image1Url,
            Image2Url = request.Image2Url,
            Image3Url = request.Image3Url,
            DisplayOrder = request.DisplayOrder,
            IsActive = request.IsActive,
            IsAvailable = request.IsAvailable,
            IsNew = request.IsNew,
            IsPopular = request.IsPopular,
            IsSpicy = request.IsSpicy,
            IsVegetarian = request.IsVegetarian,
            StockQuantity = request.StockQuantity,
            PreparationTime = request.PreparationTime,
            Options = request.Options,
            NutritionalInfo = request.NutritionalInfo
        };

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}

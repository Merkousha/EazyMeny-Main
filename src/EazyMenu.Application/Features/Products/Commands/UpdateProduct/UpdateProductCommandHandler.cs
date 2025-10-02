using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// Handler برای ویرایش محصول
/// </summary>
public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(
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

    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        // بررسی وجود محصول
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            throw new Exception($"محصول با شناسه {request.Id} یافت نشد.");

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

        // به‌روزرسانی فیلدها
        product.RestaurantId = request.RestaurantId;
        product.CategoryId = request.CategoryId;
        product.Name = request.Name;
        product.NameEn = request.NameEn;
        product.Description = request.Description;
        product.Price = request.Price;
        product.DiscountedPrice = request.DiscountedPrice;
        product.Image1Url = request.Image1Url;
        product.Image2Url = request.Image2Url;
        product.Image3Url = request.Image3Url;
        product.DisplayOrder = request.DisplayOrder;
        product.IsActive = request.IsActive;
        product.IsAvailable = request.IsAvailable;
        product.IsNew = request.IsNew;
        product.IsPopular = request.IsPopular;
        product.IsSpicy = request.IsSpicy;
        product.IsVegetarian = request.IsVegetarian;
        product.StockQuantity = request.StockQuantity;
        product.PreparationTime = request.PreparationTime;
        product.Options = request.Options;
        product.NutritionalInfo = request.NutritionalInfo;

        await _productRepository.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

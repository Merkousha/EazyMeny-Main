using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Categories.Commands.DeleteCategory;

/// <summary>
/// Handler برای حذف دسته‌بندی (Soft Delete)
/// </summary>
public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(
        IRepository<Category> categoryRepository,
        IRepository<Product> productRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        // بررسی وجود دسته‌بندی
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            throw new ArgumentException("دسته‌بندی مورد نظر یافت نشد");
        }

        // بررسی محصولات مرتبط
        var products = await _productRepository.GetAllAsync(cancellationToken);
        var hasProducts = products.Any(p => p.CategoryId == request.Id);

        if (hasProducts)
        {
            throw new InvalidOperationException("امکان حذف دسته‌بندی با محصولات موجود وجود ندارد. ابتدا محصولات را حذف کنید.");
        }

        // Soft Delete
        await _categoryRepository.DeleteAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

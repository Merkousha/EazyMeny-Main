using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Products.Commands.DeleteProduct;

/// <summary>
/// Handler برای حذف محصول (Soft Delete)
/// </summary>
public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(
        IRepository<Product> productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
            throw new Exception($"محصول با شناسه {request.Id} یافت نشد.");

        // Soft Delete - فقط IsDeleted را true می‌کنیم
        // محصولات در سفارش‌های قبلی باقی می‌مانند
        await _productRepository.DeleteAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

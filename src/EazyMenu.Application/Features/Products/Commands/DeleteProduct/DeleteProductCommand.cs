using MediatR;

namespace EazyMenu.Application.Features.Products.Commands.DeleteProduct;

/// <summary>
/// Command برای حذف محصول (Soft Delete)
/// </summary>
public class DeleteProductCommand : IRequest<Unit>
{
    public Guid Id { get; set; }

    public DeleteProductCommand(Guid id)
    {
        Id = id;
    }
}

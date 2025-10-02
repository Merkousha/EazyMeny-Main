using EazyMenu.Domain.Enums;
using MediatR;

namespace EazyMenu.Application.Features.Orders.Commands.UpdateOrderStatus;

/// <summary>
/// Command برای تغییر وضعیت سفارش
/// </summary>
public record UpdateOrderStatusCommand(
    Guid OrderId,
    OrderStatus NewStatus,
    string? CancellationReason = null
) : IRequest<bool>;

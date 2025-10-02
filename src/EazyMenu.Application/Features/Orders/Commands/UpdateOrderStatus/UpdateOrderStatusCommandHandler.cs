using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using EazyMenu.Domain.Enums;
using MediatR;

namespace EazyMenu.Application.Features.Orders.Commands.UpdateOrderStatus;

/// <summary>
/// Handler برای تغییر وضعیت سفارش
/// </summary>
public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, bool>
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderStatusCommandHandler(
        IRepository<Order> orderRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        
        if (order == null)
            return false;

        // بروزرسانی وضعیت
        order.Status = request.NewStatus;

        // اگر وضعیت لغو شد، دلیل را ثبت کن
        if (request.NewStatus == OrderStatus.Cancelled && !string.IsNullOrWhiteSpace(request.CancellationReason))
        {
            order.CancellationReason = request.CancellationReason;
        }

        // ثبت زمان‌های مختلف
        if (request.NewStatus == OrderStatus.Ready && !order.PreparedAt.HasValue)
        {
            order.PreparedAt = DateTime.Now;
        }
        
        if (request.NewStatus == OrderStatus.Delivered && !order.DeliveredAt.HasValue)
        {
            order.DeliveredAt = DateTime.Now;
        }

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

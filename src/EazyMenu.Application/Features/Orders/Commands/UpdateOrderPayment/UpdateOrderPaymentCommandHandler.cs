using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using EazyMenu.Domain.Enums;
using MediatR;

namespace EazyMenu.Application.Features.Orders.Commands.UpdateOrderPayment;

/// <summary>
/// Handler برای بروزرسانی وضعیت پرداخت
/// </summary>
public class UpdateOrderPaymentCommandHandler : IRequestHandler<UpdateOrderPaymentCommand, bool>
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderPaymentCommandHandler(
        IRepository<Order> orderRepository,
        IRepository<Payment> paymentRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateOrderPaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        
        if (order == null)
            return false;

        // ایجاد Payment entity
        var payment = new Payment
        {
            OrderId = request.OrderId,
            Authority = request.PaymentAuthority ?? string.Empty,
            RefID = request.PaymentRefId,
            Amount = order.TotalAmount,
            IsSubscriptionPayment = false,
            IsSuccessful = request.IsPaid,
            PaidAt = request.IsPaid ? DateTime.Now : null
        };

        await _paymentRepository.AddAsync(payment, cancellationToken);

        // بروزرسانی وضعیت سفارش
        order.IsPaid = request.IsPaid;
        order.IsOnlinePayment = true;
        order.PaymentId = payment.Id;
        
        // اگر پرداخت موفق بود، وضعیت را به Confirmed تغییر بده
        if (request.IsPaid)
        {
            order.Status = OrderStatus.Confirmed;
        }

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

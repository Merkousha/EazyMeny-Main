using MediatR;

namespace EazyMenu.Application.Features.Orders.Commands.UpdateOrderPayment;

/// <summary>
/// Command برای بروزرسانی وضعیت پرداخت سفارش
/// </summary>
public class UpdateOrderPaymentCommand : IRequest<bool>
{
    public Guid OrderId { get; set; }
    public bool IsPaid { get; set; }
    public string? PaymentAuthority { get; set; }
    public string? PaymentRefId { get; set; }
}

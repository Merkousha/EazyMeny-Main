using MediatR;

namespace EazyMenu.Application.Features.Subscriptions.Commands.VerifyPayment;

/// <summary>
/// Command برای تایید پرداخت و فعال‌سازی اشتراک
/// </summary>
public class VerifyPaymentCommand : IRequest<VerifyPaymentResult>
{
    /// <summary>
    /// Authority از زرین‌پال
    /// </summary>
    public string Authority { get; set; } = string.Empty;

    /// <summary>
    /// Status از Callback زرین‌پال (OK یا NOK)
    /// </summary>
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// نتیجه تایید پرداخت
/// </summary>
public class VerifyPaymentResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public long? RefID { get; set; }
    public Guid? SubscriptionId { get; set; }
    public decimal Amount { get; set; }
}

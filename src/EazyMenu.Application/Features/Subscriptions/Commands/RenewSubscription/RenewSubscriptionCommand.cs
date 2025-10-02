using MediatR;

namespace EazyMenu.Application.Features.Subscriptions.Commands.RenewSubscription;

/// <summary>
/// Command برای تمدید اشتراک موجود
/// </summary>
public class RenewSubscriptionCommand : IRequest<RenewSubscriptionResult>
{
    /// <summary>
    /// شناسه اشتراک برای تمدید
    /// </summary>
    public Guid SubscriptionId { get; set; }

    /// <summary>
    /// آیا پرداخت سالانه است؟
    /// </summary>
    public bool IsYearly { get; set; }

    /// <summary>
    /// URL بازگشت بعد از پرداخت
    /// </summary>
    public string CallbackUrl { get; set; } = string.Empty;
}

/// <summary>
/// نتیجه تمدید اشتراک
/// </summary>
public class RenewSubscriptionResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Authority { get; set; }
    public string? PaymentUrl { get; set; }
    public Guid? PaymentId { get; set; }
}

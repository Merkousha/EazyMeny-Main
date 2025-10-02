using MediatR;

namespace EazyMenu.Application.Features.Subscriptions.Commands.PurchaseSubscription;

/// <summary>
/// Command برای خرید اشتراک جدید
/// </summary>
public class PurchaseSubscriptionCommand : IRequest<PurchaseSubscriptionResult>
{
    /// <summary>
    /// شناسه پلن انتخاب شده
    /// </summary>
    public Guid SubscriptionPlanId { get; set; }

    /// <summary>
    /// شناسه رستوران (از Context یا User گرفته می‌شود)
    /// </summary>
    public Guid RestaurantId { get; set; }

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
/// نتیجه خرید اشتراک
/// </summary>
public class PurchaseSubscriptionResult
{
    /// <summary>
    /// موفقیت‌آمیز بودن
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// پیام خطا
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Authority زرین‌پال برای Redirect
    /// </summary>
    public string? Authority { get; set; }

    /// <summary>
    /// URL بازگشت کامل
    /// </summary>
    public string? PaymentUrl { get; set; }

    /// <summary>
    /// شناسه اشتراک ایجاد شده
    /// </summary>
    public Guid? SubscriptionId { get; set; }

    /// <summary>
    /// شناسه پرداخت ایجاد شده
    /// </summary>
    public Guid? PaymentId { get; set; }
}

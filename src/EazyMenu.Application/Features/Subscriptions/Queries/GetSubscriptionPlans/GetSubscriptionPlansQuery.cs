using EazyMenu.Application.Common.Models.Subscription;
using MediatR;

namespace EazyMenu.Application.Features.Subscriptions.Queries.GetSubscriptionPlans;

/// <summary>
/// Query برای دریافت لیست پلن‌های اشتراک
/// </summary>
public class GetSubscriptionPlansQuery : IRequest<List<SubscriptionPlanDto>>
{
    /// <summary>
    /// فقط پلن‌های فعال
    /// </summary>
    public bool OnlyActive { get; set; } = true;
}

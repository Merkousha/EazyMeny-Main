using EazyMenu.Application.Common.Models.Subscription;
using MediatR;

namespace EazyMenu.Application.Features.Subscriptions.Queries.GetAllSubscriptions;

/// <summary>
/// Query برای دریافت لیست اشتراک‌ها
/// </summary>
public class GetAllSubscriptionsQuery : IRequest<List<SubscriptionListDto>>
{
    public Guid? RestaurantId { get; init; }
    public string? Status { get; init; }
}

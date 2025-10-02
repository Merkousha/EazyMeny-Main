using System;
using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Subscription;
using EazyMenu.Domain.Entities;
using EazyMenu.Domain.Enums;
using MediatR;

namespace EazyMenu.Application.Features.Subscriptions.Queries.GetAllSubscriptions;

/// <summary>
/// Handler برای دریافت لیست اشتراک‌ها
/// </summary>
public class GetAllSubscriptionsQueryHandler : IRequestHandler<GetAllSubscriptionsQuery, List<SubscriptionListDto>>
{
    private readonly IRepository<Domain.Entities.Subscription> _subscriptionRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;

    public GetAllSubscriptionsQueryHandler(
        IRepository<Domain.Entities.Subscription> subscriptionRepository,
        IRepository<Restaurant> restaurantRepository)
    {
        _subscriptionRepository = subscriptionRepository;
        _restaurantRepository = restaurantRepository;
    }

    public async Task<List<SubscriptionListDto>> Handle(GetAllSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        // دریافت تمام اشتراک‌ها با Include SubscriptionPlan
        var allSubscriptions = await _subscriptionRepository.FindWithIncludesAsync(
            s => true,
            cancellationToken,
            s => s.SubscriptionPlan);
        
        var subscriptions = allSubscriptions.AsEnumerable();

        if (request.RestaurantId.HasValue)
        {
            subscriptions = subscriptions.Where(s => s.RestaurantId == request.RestaurantId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<SubscriptionStatus>(request.Status, true, out var statusFilter))
        {
            subscriptions = subscriptions.Where(s => s.Status == statusFilter);
        }

        // کش کردن نام رستوران‌ها
        var restaurantIds = subscriptions.Select(s => s.RestaurantId).Distinct().ToList();
        var restaurants = await _restaurantRepository.FindAsync(r => restaurantIds.Contains(r.Id), cancellationToken);
        var restaurantLookup = restaurants.ToDictionary(r => r.Id, r => r.Name);

        var result = subscriptions
            .OrderByDescending(s => s.CreatedAt)
            .Select(subscription => new SubscriptionListDto
            {
                Id = subscription.Id,
                RestaurantId = subscription.RestaurantId,
                RestaurantName = restaurantLookup.TryGetValue(subscription.RestaurantId, out var name) ? name : "-",
                Plan = subscription.SubscriptionPlan?.Name ?? "نامشخص",
                Status = GetStatusTitle(subscription.Status),
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                Amount = subscription.Amount,
                IsActive = subscription.Status == SubscriptionStatus.Active || subscription.Status == SubscriptionStatus.Trial,
                DaysRemaining = (subscription.EndDate - DateTime.UtcNow).Days
            })
            .ToList();

        return result;
    }

    private static string GetPlanTitle(PlanType plan) => plan switch
    {
        PlanType.Basic => "پایه",
        PlanType.Standard => "استاندارد",
        PlanType.Premium => "پیشرفته",
        _ => plan.ToString()
    };

    private static string GetStatusTitle(SubscriptionStatus status) => status switch
    {
        SubscriptionStatus.Trial => "آزمایشی",
        SubscriptionStatus.Active => "فعال",
        SubscriptionStatus.Expiring => "در حال انقضا",
        SubscriptionStatus.Expired => "منقضی شده",
        SubscriptionStatus.Suspended => "معلق",
        SubscriptionStatus.Cancelled => "لغو شده",
        _ => status.ToString()
    };
}

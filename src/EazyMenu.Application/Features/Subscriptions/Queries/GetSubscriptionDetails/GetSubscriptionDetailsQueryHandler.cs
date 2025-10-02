using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Subscription;
using EazyMenu.Domain.Entities;
using EazyMenu.Domain.Enums;
using MediatR;

namespace EazyMenu.Application.Features.Subscriptions.Queries.GetSubscriptionDetails;

/// <summary>
/// Handler برای دریافت جزئیات اشتراک
/// </summary>
public class GetSubscriptionDetailsQueryHandler : IRequestHandler<GetSubscriptionDetailsQuery, SubscriptionDetailsDto?>
{
    private readonly IRepository<Domain.Entities.Subscription> _subscriptionRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;

    public GetSubscriptionDetailsQueryHandler(
        IRepository<Domain.Entities.Subscription> subscriptionRepository,
        IRepository<Restaurant> restaurantRepository)
    {
        _subscriptionRepository = subscriptionRepository;
        _restaurantRepository = restaurantRepository;
    }

    public async Task<SubscriptionDetailsDto?> Handle(GetSubscriptionDetailsQuery request, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (subscription == null)
            return null;

        var restaurant = await _restaurantRepository.GetByIdAsync(subscription.RestaurantId, cancellationToken);

        return new SubscriptionDetailsDto
        {
            Id = subscription.Id,
            RestaurantId = subscription.RestaurantId,
            RestaurantName = restaurant?.Name ?? "-",
            RestaurantPhone = restaurant?.PhoneNumber ?? "-",
            Plan = GetPlanTitle(subscription.Plan),
            Status = GetStatusTitle(subscription.Status),
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            Amount = subscription.Amount,
            AutoRenew = subscription.AutoRenew,
            CreatedAt = subscription.CreatedAt,
            UpdatedAt = subscription.UpdatedAt ?? subscription.CreatedAt,
            IsActive = subscription.Status == SubscriptionStatus.Active || subscription.Status == SubscriptionStatus.Trial,
            DaysRemaining = (subscription.EndDate - DateTime.UtcNow).Days
        };
    }

    private static string GetPlanTitle(SubscriptionPlan plan) => plan switch
    {
        SubscriptionPlan.Basic => "پایه",
        SubscriptionPlan.Standard => "استاندارد",
        SubscriptionPlan.Premium => "پیشرفته",
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

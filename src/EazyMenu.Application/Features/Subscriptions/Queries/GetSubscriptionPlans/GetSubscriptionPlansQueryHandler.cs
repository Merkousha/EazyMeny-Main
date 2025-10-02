using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Subscription;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Subscriptions.Queries.GetSubscriptionPlans;

/// <summary>
/// Handler برای دریافت لیست پلن‌های اشتراک
/// </summary>
public class GetSubscriptionPlansQueryHandler : IRequestHandler<GetSubscriptionPlansQuery, List<SubscriptionPlanDto>>
{
    private readonly IRepository<SubscriptionPlan> _planRepository;

    public GetSubscriptionPlansQueryHandler(IRepository<SubscriptionPlan> planRepository)
    {
        _planRepository = planRepository;
    }

    public async Task<List<SubscriptionPlanDto>> Handle(GetSubscriptionPlansQuery request, CancellationToken cancellationToken)
    {
        // دریافت تمام پلن‌ها
        var plans = await _planRepository.FindAsync(
            p => !request.OnlyActive || p.IsActive,
            cancellationToken);

        // مرتب‌سازی بر اساس DisplayOrder
        var sortedPlans = plans
            .OrderBy(p => p.DisplayOrder)
            .Select(plan => new SubscriptionPlanDto
            {
                Id = plan.Id,
                PlanType = plan.PlanType,
                Name = plan.Name,
                Description = plan.Description ?? string.Empty,
                PriceMonthly = plan.PriceMonthly,
                PriceYearly = plan.PriceYearly,
                MaxProducts = plan.MaxProducts,
                MaxCategories = plan.MaxCategories,
                MaxOrders = plan.MaxOrders,
                HasQRCode = plan.HasQRCode,
                HasWebsite = plan.HasWebsite,
                HasReservation = plan.HasReservation,
                HasAnalytics = plan.HasAnalytics,
                SupportLevel = plan.SupportLevel,
                Features = plan.Features,
                DisplayOrder = plan.DisplayOrder,
                IsActive = plan.IsActive,
                IsPopular = plan.IsPopular
            })
            .ToList();

        return sortedPlans;
    }
}

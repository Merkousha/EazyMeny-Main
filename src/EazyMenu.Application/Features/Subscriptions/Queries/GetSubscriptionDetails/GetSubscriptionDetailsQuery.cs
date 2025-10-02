using EazyMenu.Application.Common.Models.Subscription;
using MediatR;

namespace EazyMenu.Application.Features.Subscriptions.Queries.GetSubscriptionDetails;

/// <summary>
/// Query برای دریافت جزئیات اشتراک
/// </summary>
public record GetSubscriptionDetailsQuery(Guid Id) : IRequest<SubscriptionDetailsDto?>;

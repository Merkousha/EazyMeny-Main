using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using EazyMenu.Domain.Enums;
using MediatR;

namespace EazyMenu.Application.Features.Subscriptions.Commands.PurchaseSubscription;

/// <summary>
/// Handler برای خرید اشتراک جدید
/// </summary>
public class PurchaseSubscriptionCommandHandler : IRequestHandler<PurchaseSubscriptionCommand, PurchaseSubscriptionResult>
{
    private readonly IRepository<SubscriptionPlan> _planRepository;
    private readonly IRepository<Domain.Entities.Subscription> _subscriptionRepository;
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IPaymentService _paymentService;
    private readonly IUnitOfWork _unitOfWork;

    public PurchaseSubscriptionCommandHandler(
        IRepository<SubscriptionPlan> planRepository,
        IRepository<Domain.Entities.Subscription> subscriptionRepository,
        IRepository<Payment> paymentRepository,
        IRepository<Restaurant> restaurantRepository,
        IPaymentService paymentService,
        IUnitOfWork unitOfWork)
    {
        _planRepository = planRepository;
        _subscriptionRepository = subscriptionRepository;
        _paymentRepository = paymentRepository;
        _restaurantRepository = restaurantRepository;
        _paymentService = paymentService;
        _unitOfWork = unitOfWork;
    }

    public async Task<PurchaseSubscriptionResult> Handle(PurchaseSubscriptionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. بررسی وجود پلن
            var plan = await _planRepository.GetByIdAsync(request.SubscriptionPlanId, cancellationToken);
            if (plan == null || !plan.IsActive)
            {
                return new PurchaseSubscriptionResult
                {
                    Success = false,
                    ErrorMessage = "پلن اشتراک یافت نشد یا غیرفعال است"
                };
            }

            // 2. بررسی وجود رستوران
            var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId, cancellationToken);
            if (restaurant == null)
            {
                return new PurchaseSubscriptionResult
                {
                    Success = false,
                    ErrorMessage = "رستوران یافت نشد"
                };
            }

            // 3. بررسی اشتراک فعال موجود
            var existingSubscriptions = await _subscriptionRepository.FindAsync(
                s => s.RestaurantId == request.RestaurantId &&
                     (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trial),
                cancellationToken);

            if (existingSubscriptions.Any())
            {
                return new PurchaseSubscriptionResult
                {
                    Success = false,
                    ErrorMessage = "رستوران دارای اشتراک فعال است. لطفاً ابتدا اشتراک فعلی را لغو کنید."
                };
            }

            // 4. محاسبه مبلغ
            var amount = request.IsYearly ? plan.PriceYearly : plan.PriceMonthly;

            // 5. ایجاد Subscription با Status=Trial (تا پرداخت تایید شود)
            var subscription = new Domain.Entities.Subscription
            {
                Id = Guid.NewGuid(),
                RestaurantId = request.RestaurantId,
                SubscriptionPlanId = plan.Id,
                Status = SubscriptionStatus.Trial, // موقتاً Trial تا پرداخت تایید شود
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(request.IsYearly ? 365 : 30),
                Amount = amount,
                IsYearly = request.IsYearly,
                AutoRenew = false,
                MaxProducts = plan.MaxProducts,
                MaxOrdersPerMonth = plan.MaxOrders,
                HasReservationFeature = plan.HasReservation,
                HasWebsiteBuilder = plan.HasWebsite,
                HasAdvancedReporting = plan.HasAnalytics,
                CurrentProductCount = 0,
                CurrentMonthOrderCount = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _subscriptionRepository.AddAsync(subscription, cancellationToken);

            // 6. ایجاد Payment
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                TransactionId = Guid.NewGuid().ToString("N"),
                Authority = string.Empty, // بعد از درخواست زرین‌پال پر می‌شود
                Amount = amount,
                IsSubscriptionPayment = true,
                SubscriptionId = subscription.Id,
                OrderId = null,
                IsSuccessful = false,
                CreatedAt = DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 7. درخواست پرداخت از زرین‌پال
            var paymentResult = await _paymentService.RequestPaymentAsync(
                amount,
                $"خرید اشتراک {plan.Name} - {restaurant.Name}",
                request.CallbackUrl,
                cancellationToken);

            if (!paymentResult.IsSuccess || string.IsNullOrEmpty(paymentResult.Authority))
            {
                return new PurchaseSubscriptionResult
                {
                    Success = false,
                    ErrorMessage = paymentResult.ErrorMessage ?? "خطا در ایجاد درخواست پرداخت"
                };
            }

            // 8. به‌روزرسانی Authority در Payment
            payment.Authority = paymentResult.Authority;
            await _paymentRepository.UpdateAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 9. بازگشت نتیجه موفق
            return new PurchaseSubscriptionResult
            {
                Success = true,
                Authority = paymentResult.Authority,
                PaymentUrl = paymentResult.PaymentUrl,
                SubscriptionId = subscription.Id,
                PaymentId = payment.Id
            };
        }
        catch (Exception ex)
        {
            return new PurchaseSubscriptionResult
            {
                Success = false,
                ErrorMessage = $"خطا در پردازش درخواست: {ex.Message}"
            };
        }
    }
}

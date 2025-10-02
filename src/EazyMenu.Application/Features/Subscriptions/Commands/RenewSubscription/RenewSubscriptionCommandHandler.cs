using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Subscriptions.Commands.RenewSubscription;

/// <summary>
/// Handler برای تمدید اشتراک
/// </summary>
public class RenewSubscriptionCommandHandler : IRequestHandler<RenewSubscriptionCommand, RenewSubscriptionResult>
{
    private readonly IRepository<Domain.Entities.Subscription> _subscriptionRepository;
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IPaymentService _paymentService;
    private readonly IUnitOfWork _unitOfWork;

    public RenewSubscriptionCommandHandler(
        IRepository<Domain.Entities.Subscription> subscriptionRepository,
        IRepository<Payment> paymentRepository,
        IRepository<Restaurant> restaurantRepository,
        IPaymentService paymentService,
        IUnitOfWork unitOfWork)
    {
        _subscriptionRepository = subscriptionRepository;
        _paymentRepository = paymentRepository;
        _restaurantRepository = restaurantRepository;
        _paymentService = paymentService;
        _unitOfWork = unitOfWork;
    }

    public async Task<RenewSubscriptionResult> Handle(RenewSubscriptionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. دریافت اشتراک با Include SubscriptionPlan
            var subscription = await _subscriptionRepository.GetByIdWithIncludesAsync(
                request.SubscriptionId,
                cancellationToken,
                s => s.SubscriptionPlan);

            if (subscription == null)
            {
                return new RenewSubscriptionResult
                {
                    Success = false,
                    ErrorMessage = "اشتراک یافت نشد"
                };
            }

            if (subscription.SubscriptionPlan == null)
            {
                return new RenewSubscriptionResult
                {
                    Success = false,
                    ErrorMessage = "پلن اشتراک یافت نشد"
                };
            }

            // 2. دریافت رستوران
            var restaurant = await _restaurantRepository.GetByIdAsync(subscription.RestaurantId, cancellationToken);
            if (restaurant == null)
            {
                return new RenewSubscriptionResult
                {
                    Success = false,
                    ErrorMessage = "رستوران یافت نشد"
                };
            }

            // 3. محاسبه مبلغ
            var amount = request.IsYearly
                ? subscription.SubscriptionPlan.PriceYearly
                : subscription.SubscriptionPlan.PriceMonthly;

            // 4. ایجاد Payment
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                TransactionId = Guid.NewGuid().ToString("N"),
                Authority = string.Empty,
                Amount = amount,
                IsSubscriptionPayment = true,
                SubscriptionId = subscription.Id,
                OrderId = null,
                IsSuccessful = false,
                CreatedAt = DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 5. درخواست پرداخت از زرین‌پال
            var paymentResult = await _paymentService.RequestPaymentAsync(
                amount,
                $"تمدید اشتراک {subscription.SubscriptionPlan.Name} - {restaurant.Name}",
                request.CallbackUrl,
                cancellationToken);

            if (!paymentResult.IsSuccess || string.IsNullOrEmpty(paymentResult.Authority))
            {
                return new RenewSubscriptionResult
                {
                    Success = false,
                    ErrorMessage = paymentResult.ErrorMessage ?? "خطا در ایجاد درخواست پرداخت"
                };
            }

            // 6. به‌روزرسانی Authority
            payment.Authority = paymentResult.Authority;
            await _paymentRepository.UpdateAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 7. بازگشت نتیجه موفق
            return new RenewSubscriptionResult
            {
                Success = true,
                Authority = paymentResult.Authority,
                PaymentUrl = paymentResult.PaymentUrl,
                PaymentId = payment.Id
            };
        }
        catch (Exception ex)
        {
            return new RenewSubscriptionResult
            {
                Success = false,
                ErrorMessage = $"خطا در پردازش درخواست: {ex.Message}"
            };
        }
    }
}

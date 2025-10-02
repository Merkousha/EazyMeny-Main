using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using EazyMenu.Domain.Enums;
using MediatR;

namespace EazyMenu.Application.Features.Subscriptions.Commands.VerifyPayment;

/// <summary>
/// Handler برای تایید پرداخت و فعال‌سازی اشتراک
/// </summary>
public class VerifyPaymentCommandHandler : IRequestHandler<VerifyPaymentCommand, VerifyPaymentResult>
{
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IRepository<Domain.Entities.Subscription> _subscriptionRepository;
    private readonly IPaymentService _paymentService;
    private readonly IUnitOfWork _unitOfWork;

    public VerifyPaymentCommandHandler(
        IRepository<Payment> paymentRepository,
        IRepository<Domain.Entities.Subscription> subscriptionRepository,
        IPaymentService paymentService,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _subscriptionRepository = subscriptionRepository;
        _paymentService = paymentService;
        _unitOfWork = unitOfWork;
    }

    public async Task<VerifyPaymentResult> Handle(VerifyPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. بررسی Status از Callback
            if (request.Status != "OK")
            {
                return new VerifyPaymentResult
                {
                    Success = false,
                    ErrorMessage = "پرداخت توسط کاربر لغو شد"
                };
            }

            // 2. یافتن Payment بر اساس Authority
            var payments = await _paymentRepository.FindAsync(
                p => p.Authority == request.Authority,
                cancellationToken);

            var payment = payments.FirstOrDefault();
            if (payment == null)
            {
                return new VerifyPaymentResult
                {
                    Success = false,
                    ErrorMessage = "اطلاعات پرداخت یافت نشد"
                };
            }

            // 3. بررسی اینکه قبلاً تایید نشده باشد
            if (payment.IsSuccessful)
            {
                return new VerifyPaymentResult
                {
                    Success = false,
                    ErrorMessage = "این پرداخت قبلاً تایید شده است"
                };
            }

            // 4. تایید پرداخت از زرین‌پال
            var verifyResult = await _paymentService.VerifyPaymentAsync(
                request.Authority,
                payment.Amount,
                cancellationToken);

            if (!verifyResult.IsSuccess)
            {
                payment.ErrorMessage = verifyResult.ErrorMessage;
                await _paymentRepository.UpdateAsync(payment, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new VerifyPaymentResult
                {
                    Success = false,
                    ErrorMessage = verifyResult.ErrorMessage ?? "خطا در تایید پرداخت"
                };
            }

            // 5. به‌روزرسانی Payment
            payment.IsSuccessful = true;
            payment.PaidAt = DateTime.UtcNow;
            payment.RefID = verifyResult.RefID?.ToString();
            payment.CardPan = verifyResult.CardPan;
            await _paymentRepository.UpdateAsync(payment, cancellationToken);

            // 6. فعال‌سازی اشتراک (اگر پرداخت Subscription باشد)
            if (payment.IsSubscriptionPayment && payment.SubscriptionId.HasValue)
            {
                var subscription = await _subscriptionRepository.GetByIdAsync(
                    payment.SubscriptionId.Value,
                    cancellationToken);

                if (subscription != null)
                {
                    // اگر Trial بود، به Active تبدیل می‌شود
                    if (subscription.Status == SubscriptionStatus.Trial)
                    {
                        subscription.Status = SubscriptionStatus.Active;
                        subscription.StartDate = DateTime.UtcNow;
                    }
                    // اگر Expired یا Expiring بود، تمدید می‌شود
                    else if (subscription.Status == SubscriptionStatus.Expired ||
                             subscription.Status == SubscriptionStatus.Expiring)
                    {
                        subscription.Status = SubscriptionStatus.Active;
                        // تمدید از تاریخ فعلی
                        subscription.EndDate = DateTime.UtcNow.AddDays(subscription.IsYearly ? 365 : 30);
                    }

                    subscription.UpdatedAt = DateTime.UtcNow;
                    await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 7. بازگشت نتیجه موفق
            return new VerifyPaymentResult
            {
                Success = true,
                RefID = verifyResult.RefID,
                SubscriptionId = payment.SubscriptionId,
                Amount = payment.Amount
            };
        }
        catch (Exception ex)
        {
            return new VerifyPaymentResult
            {
                Success = false,
                ErrorMessage = $"خطا در پردازش درخواست: {ex.Message}"
            };
        }
    }
}

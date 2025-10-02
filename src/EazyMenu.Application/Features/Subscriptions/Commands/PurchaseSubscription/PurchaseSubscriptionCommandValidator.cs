using FluentValidation;

namespace EazyMenu.Application.Features.Subscriptions.Commands.PurchaseSubscription;

/// <summary>
/// Validator برای PurchaseSubscriptionCommand
/// </summary>
public class PurchaseSubscriptionCommandValidator : AbstractValidator<PurchaseSubscriptionCommand>
{
    public PurchaseSubscriptionCommandValidator()
    {
        RuleFor(x => x.SubscriptionPlanId)
            .NotEmpty().WithMessage("پلن اشتراک الزامی است");

        RuleFor(x => x.RestaurantId)
            .NotEmpty().WithMessage("شناسه رستوران الزامی است");

        RuleFor(x => x.CallbackUrl)
            .NotEmpty().WithMessage("آدرس بازگشت الزامی است")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("آدرس بازگشت معتبر نیست");
    }
}

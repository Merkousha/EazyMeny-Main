using FluentValidation;

namespace EazyMenu.Application.Features.Orders.Commands.CreateOrder;

/// <summary>
/// Validator برای CreateOrderCommand
/// </summary>
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.RestaurantId)
            .NotEmpty().WithMessage("رستوران انتخاب نشده است.");

        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("نام مشتری الزامی است.")
            .MaximumLength(100).WithMessage("نام مشتری نباید بیش از 100 کاراکتر باشد.");

        RuleFor(x => x.CustomerPhone)
            .NotEmpty().WithMessage("شماره تماس الزامی است.")
            .Matches(@"^09\d{9}$").WithMessage("شماره تماس باید با فرمت 09xxxxxxxxx باشد.");

        RuleFor(x => x.DeliveryAddress)
            .NotEmpty().When(x => !x.IsTakeaway)
            .WithMessage("آدرس تحویل برای سفارش پیک الزامی است.")
            .MaximumLength(500).WithMessage("آدرس نباید بیش از 500 کاراکتر باشد.");

        RuleFor(x => x.Note)
            .MaximumLength(1000).When(x => !string.IsNullOrEmpty(x.Note))
            .WithMessage("یادداشت نباید بیش از 1000 کاراکتر باشد.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("سبد خرید خالی است. لطفاً محصولاتی را اضافه کنید.")
            .Must(items => items.Count > 0).WithMessage("حداقل یک محصول باید در سفارش باشد.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("شناسه محصول نامعتبر است.");

            item.RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("تعداد باید حداقل 1 باشد.")
                .LessThanOrEqualTo(99).WithMessage("حداکثر تعداد مجاز 99 عدد است.");

            item.RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("قیمت محصول نامعتبر است.");
        });
    }
}

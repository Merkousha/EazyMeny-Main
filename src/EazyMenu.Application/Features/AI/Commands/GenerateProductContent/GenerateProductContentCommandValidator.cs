using FluentValidation;

namespace EazyMenu.Application.Features.AI.Commands.GenerateProductContent;

/// <summary>
/// اعتبارسنج دستور تولید محتوای محصول
/// </summary>
public class GenerateProductContentCommandValidator : AbstractValidator<GenerateProductContentCommand>
{
    public GenerateProductContentCommandValidator()
    {
        RuleFor(x => x.RestaurantId)
            .NotEmpty()
            .WithMessage("شناسه رستوران الزامی است.");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("شناسه محصول الزامی است.");

        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("نام محصول الزامی است.")
            .MaximumLength(200)
            .WithMessage("نام محصول نباید بیشتر از 200 کاراکتر باشد.");

        RuleFor(x => x.Ingredients)
            .NotEmpty()
            .WithMessage("مواد تشکیل‌دهنده الزامی است.")
            .MaximumLength(1000)
            .WithMessage("مواد تشکیل‌دهنده نباید بیشتر از 1000 کاراکتر باشد.");

        RuleFor(x => x.Tone)
            .Must(tone => new[] { "رسمی", "صمیمی", "خلاقانه" }.Contains(tone))
            .WithMessage("لحن باید یکی از مقادیر 'رسمی'، 'صمیمی' یا 'خلاقانه' باشد.");
    }
}

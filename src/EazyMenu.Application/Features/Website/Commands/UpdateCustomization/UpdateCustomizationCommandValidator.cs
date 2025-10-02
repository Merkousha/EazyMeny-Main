using FluentValidation;

namespace EazyMenu.Application.Features.Website.Commands.UpdateCustomization;

/// <summary>
/// Validator برای UpdateCustomizationCommand
/// </summary>
public class UpdateCustomizationCommandValidator : AbstractValidator<UpdateCustomizationCommand>
{
    public UpdateCustomizationCommandValidator()
    {
        RuleFor(x => x.RestaurantId)
            .NotEmpty().WithMessage("شناسه رستوران الزامی است");

        RuleFor(x => x.PrimaryColor)
            .NotEmpty().WithMessage("رنگ اصلی الزامی است")
            .Matches(@"^#[0-9A-Fa-f]{6}$").WithMessage("فرمت رنگ نامعتبر است (مثال: #FF6B35)");

        RuleFor(x => x.SecondaryColor)
            .NotEmpty().WithMessage("رنگ ثانویه الزامی است")
            .Matches(@"^#[0-9A-Fa-f]{6}$").WithMessage("فرمت رنگ نامعتبر است");

        RuleFor(x => x.TextColor)
            .NotEmpty().WithMessage("رنگ متن الزامی است")
            .Matches(@"^#[0-9A-Fa-f]{6}$").WithMessage("فرمت رنگ نامعتبر است");

        RuleFor(x => x.BackgroundColor)
            .NotEmpty().WithMessage("رنگ پس‌زمینه الزامی است")
            .Matches(@"^#[0-9A-Fa-f]{6}$").WithMessage("فرمت رنگ نامعتبر است");

        RuleFor(x => x.FontFamily)
            .NotEmpty().WithMessage("نوع فونت الزامی است")
            .MaximumLength(50).WithMessage("نام فونت نباید بیش از 50 کاراکتر باشد");

        RuleFor(x => x.FontSize)
            .InclusiveBetween(10, 24).WithMessage("اندازه فونت باید بین 10 تا 24 پیکسل باشد");

        RuleFor(x => x.SeoTitle)
            .MaximumLength(100).WithMessage("عنوان SEO نباید بیش از 100 کاراکتر باشد");

        RuleFor(x => x.SeoDescription)
            .MaximumLength(300).WithMessage("توضیحات SEO نباید بیش از 300 کاراکتر باشد");

        RuleFor(x => x.SeoKeywords)
            .MaximumLength(200).WithMessage("کلمات کلیدی نباید بیش از 200 کاراکتر باشد");
    }
}

using FluentValidation;

namespace EazyMenu.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// Validator برای ویرایش محصول
/// </summary>
public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("شناسه محصول الزامی است.");

        RuleFor(x => x.RestaurantId)
            .NotEmpty().WithMessage("انتخاب رستوران الزامی است.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("انتخاب دسته‌بندی الزامی است.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام محصول الزامی است.")
            .MaximumLength(200).WithMessage("نام محصول نباید بیش از 200 کاراکتر باشد.");

        RuleFor(x => x.NameEn)
            .MaximumLength(200).WithMessage("نام انگلیسی نباید بیش از 200 کاراکتر باشد.")
            .When(x => !string.IsNullOrWhiteSpace(x.NameEn));

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("توضیحات نباید بیش از 1000 کاراکتر باشد.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("قیمت باید بیشتر از صفر باشد.");

        RuleFor(x => x.DiscountedPrice)
            .LessThan(x => x.Price).WithMessage("قیمت تخفیف‌خورده باید کمتر از قیمت اصلی باشد.")
            .GreaterThan(0).WithMessage("قیمت تخفیف‌خورده باید بیشتر از صفر باشد.")
            .When(x => x.DiscountedPrice.HasValue);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("ترتیب نمایش نباید منفی باشد.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("موجودی نباید منفی باشد.");

        RuleFor(x => x.PreparationTime)
            .GreaterThan(0).WithMessage("زمان آماده‌سازی باید بیشتر از صفر باشد.")
            .When(x => x.PreparationTime.HasValue);

        RuleFor(x => x.Image1Url)
            .MaximumLength(500).WithMessage("آدرس تصویر اول نباید بیش از 500 کاراکتر باشد.")
            .When(x => !string.IsNullOrWhiteSpace(x.Image1Url));

        RuleFor(x => x.Image2Url)
            .MaximumLength(500).WithMessage("آدرس تصویر دوم نباید بیش از 500 کاراکتر باشد.")
            .When(x => !string.IsNullOrWhiteSpace(x.Image2Url));

        RuleFor(x => x.Image3Url)
            .MaximumLength(500).WithMessage("آدرس تصویر سوم نباید بیش از 500 کاراکتر باشد.")
            .When(x => !string.IsNullOrWhiteSpace(x.Image3Url));
    }
}

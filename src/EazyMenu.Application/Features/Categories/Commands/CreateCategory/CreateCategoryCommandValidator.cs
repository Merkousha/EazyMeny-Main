using FluentValidation;

namespace EazyMenu.Application.Features.Categories.Commands.CreateCategory;

/// <summary>
/// Validator برای CreateCategoryCommand
/// </summary>
public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.RestaurantId)
            .NotEmpty().WithMessage("شناسه رستوران الزامی است");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام دسته‌بندی الزامی است")
            .MaximumLength(100).WithMessage("نام نباید بیش از 100 کاراکتر باشد");

        RuleFor(x => x.NameEn)
            .NotEmpty().WithMessage("نام انگلیسی الزامی است")
            .MaximumLength(100).WithMessage("نام انگلیسی نباید بیش از 100 کاراکتر باشد");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("توضیحات نباید بیش از 500 کاراکتر باشد")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.IconUrl)
            .MaximumLength(500).WithMessage("آدرس آیکون نباید بیش از 500 کاراکتر باشد")
            .When(x => !string.IsNullOrEmpty(x.IconUrl));

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("ترتیب نمایش نمی‌تواند منفی باشد");
    }
}

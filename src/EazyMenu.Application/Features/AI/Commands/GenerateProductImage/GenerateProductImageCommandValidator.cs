using FluentValidation;

namespace EazyMenu.Application.Features.AI.Commands.GenerateProductImage;

/// <summary>
/// اعتبارسنج دستور تولید تصویر محصول
/// </summary>
public class GenerateProductImageCommandValidator : AbstractValidator<GenerateProductImageCommand>
{
    public GenerateProductImageCommandValidator()
    {
        RuleFor(x => x.RestaurantId)
            .NotEmpty()
            .WithMessage("شناسه رستوران الزامی است.");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("شناسه محصول الزامی است.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("توضیحات محصول الزامی است.")
            .MaximumLength(1000)
            .WithMessage("توضیحات نباید بیشتر از 1000 کاراکتر باشد.");

        RuleFor(x => x.Style)
            .Must(style => new[] { "واقعی", "مینیمال", "هنری" }.Contains(style))
            .WithMessage("سبک تصویر باید یکی از مقادیر 'واقعی'، 'مینیمال' یا 'هنری' باشد.");

        RuleFor(x => x.Width)
            .InclusiveBetween(256, 2048)
            .WithMessage("عرض تصویر باید بین 256 تا 2048 پیکسل باشد.");

        RuleFor(x => x.Height)
            .InclusiveBetween(256, 2048)
            .WithMessage("ارتفاع تصویر باید بین 256 تا 2048 پیکسل باشد.");
    }
}

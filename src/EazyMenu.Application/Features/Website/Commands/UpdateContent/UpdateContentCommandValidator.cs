using FluentValidation;

namespace EazyMenu.Application.Features.Website.Commands.UpdateContent;

/// <summary>
/// Validator برای UpdateContentCommand
/// </summary>
public class UpdateContentCommandValidator : AbstractValidator<UpdateContentCommand>
{
    public UpdateContentCommandValidator()
    {
        RuleFor(x => x.RestaurantId)
            .NotEmpty().WithMessage("شناسه رستوران الزامی است");

        RuleFor(x => x.SectionType)
            .IsInEnum().WithMessage("نوع بخش نامعتبر است");

        RuleFor(x => x.Content)
            .NotEmpty().When(x => !x.UseDefaultContent)
            .WithMessage("محتوا الزامی است")
            .MaximumLength(20000).WithMessage("محتوا نباید بیش از 20000 کاراکتر باشد");
    }
}

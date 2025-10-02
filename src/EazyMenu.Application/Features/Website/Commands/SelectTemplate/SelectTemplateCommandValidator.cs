using FluentValidation;

namespace EazyMenu.Application.Features.Website.Commands.SelectTemplate;

/// <summary>
/// Validator برای SelectTemplateCommand
/// </summary>
public class SelectTemplateCommandValidator : AbstractValidator<SelectTemplateCommand>
{
    public SelectTemplateCommandValidator()
    {
        RuleFor(x => x.RestaurantId)
            .NotEmpty().WithMessage("شناسه رستوران الزامی است");

        RuleFor(x => x.TemplateId)
            .NotEmpty().WithMessage("شناسه قالب الزامی است");
    }
}

using FluentValidation;

namespace EazyMenu.Application.Features.Website.Commands.PublishWebsite;

/// <summary>
/// Validator برای PublishWebsiteCommand
/// </summary>
public class PublishWebsiteCommandValidator : AbstractValidator<PublishWebsiteCommand>
{
    public PublishWebsiteCommandValidator()
    {
        RuleFor(x => x.RestaurantId)
            .NotEmpty().WithMessage("شناسه رستوران الزامی است");
    }
}

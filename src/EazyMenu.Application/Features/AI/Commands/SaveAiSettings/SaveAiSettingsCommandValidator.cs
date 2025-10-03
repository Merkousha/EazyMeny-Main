using FluentValidation;

namespace EazyMenu.Application.Features.AI.Commands.SaveAiSettings;

/// <summary>
/// اعتبارسنج دستور ذخیره تنظیمات هوش مصنوعی
/// </summary>
public class SaveAiSettingsCommandValidator : AbstractValidator<SaveAiSettingsCommand>
{
    public SaveAiSettingsCommandValidator()
    {
        RuleFor(x => x.RestaurantId)
            .NotEmpty()
            .WithMessage("شناسه رستوران الزامی است.");

        RuleFor(x => x.BaseUrl)
            .NotEmpty()
            .WithMessage("آدرس پایه API الزامی است.")
            .Must(BeAValidUrl)
            .WithMessage("آدرس پایه باید یک URL معتبر باشد.");

        RuleFor(x => x.ApiKey)
            .NotEmpty()
            .WithMessage("کلید API الزامی است.")
            .MinimumLength(20)
            .WithMessage("کلید API باید حداقل 20 کاراکتر باشد.");

        RuleFor(x => x.ModelName)
            .NotEmpty()
            .WithMessage("نام مدل الزامی است.")
            .MaximumLength(100)
            .WithMessage("نام مدل نباید بیشتر از 100 کاراکتر باشد.");

        RuleFor(x => x.TimeoutSeconds)
            .InclusiveBetween(5, 300)
            .WithMessage("زمان انقضا باید بین 5 تا 300 ثانیه باشد.");

        RuleFor(x => x.Environment)
            .Must(env => new[] { "Development", "Production" }.Contains(env))
            .WithMessage("محیط باید 'Development' یا 'Production' باشد.");
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}

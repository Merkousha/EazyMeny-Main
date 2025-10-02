using FluentValidation;

namespace EazyMenu.Application.Features.Auth.Commands.Login;

/// <summary>
/// Validator برای LoginCommand
/// </summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.PhoneOrEmail)
            .NotEmpty().WithMessage("شماره موبایل یا ایمیل الزامی است");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("رمز عبور الزامی است")
            .MinimumLength(6).WithMessage("رمز عبور باید حداقل 6 کاراکتر باشد");
    }
}

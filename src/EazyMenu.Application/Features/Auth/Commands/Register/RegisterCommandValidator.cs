using FluentValidation;

namespace EazyMenu.Application.Features.Auth.Commands.Register;

/// <summary>
/// Validator برای RegisterCommand
/// </summary>
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("نام و نام خانوادگی الزامی است")
            .MinimumLength(3).WithMessage("نام باید حداقل 3 کاراکتر باشد")
            .MaximumLength(100).WithMessage("نام نباید بیش از 100 کاراکتر باشد");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("شماره موبایل الزامی است")
            .Matches(@"^09\d{9}$").WithMessage("شماره موبایل باید 11 رقم و با 09 شروع شود");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("فرمت ایمیل صحیح نیست")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("رمز عبور الزامی است")
            .MinimumLength(6).WithMessage("رمز عبور باید حداقل 6 کاراکتر باشد")
            .Matches(@"[A-Z]").WithMessage("رمز عبور باید حداقل یک حرف بزرگ داشته باشد")
            .Matches(@"[a-z]").WithMessage("رمز عبور باید حداقل یک حرف کوچک داشته باشد")
            .Matches(@"\d").WithMessage("رمز عبور باید حداقل یک عدد داشته باشد");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("تکرار رمز عبور مطابقت ندارد");

        RuleFor(x => x.AcceptTerms)
            .Equal(true).WithMessage("باید قوانین و مقررات را بپذیرید");
    }
}

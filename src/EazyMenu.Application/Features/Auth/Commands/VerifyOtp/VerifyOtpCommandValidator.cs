using FluentValidation;

namespace EazyMenu.Application.Features.Auth.Commands.VerifyOtp;

/// <summary>
/// Validator برای VerifyOtpCommand
/// </summary>
public class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("شماره موبایل الزامی است")
            .Matches(@"^09\d{9}$").WithMessage("شماره موبایل نامعتبر است");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("کد تایید الزامی است")
            .Length(5).WithMessage("کد تایید باید 5 رقم باشد")
            .Matches(@"^\d{5}$").WithMessage("کد تایید باید عددی باشد");
    }
}

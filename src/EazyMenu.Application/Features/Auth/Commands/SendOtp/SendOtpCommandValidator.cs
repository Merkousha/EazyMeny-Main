using FluentValidation;

namespace EazyMenu.Application.Features.Auth.Commands.SendOtp;

/// <summary>
/// Validator برای SendOtpCommand
/// </summary>
public class SendOtpCommandValidator : AbstractValidator<SendOtpCommand>
{
    public SendOtpCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("شماره موبایل الزامی است")
            .Matches(@"^09\d{9}$").WithMessage("شماره موبایل باید 11 رقم و با 09 شروع شود");
    }
}

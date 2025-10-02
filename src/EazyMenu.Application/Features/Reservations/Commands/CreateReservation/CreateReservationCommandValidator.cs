using FluentValidation;

namespace EazyMenu.Application.Features.Reservations.Commands.CreateReservation;

/// <summary>
/// Validator برای CreateReservationCommand
/// </summary>
public class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
{
    public CreateReservationCommandValidator()
    {
        RuleFor(x => x.RestaurantId)
            .NotEmpty()
            .WithMessage("رستوران الزامی است");
        
        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage("نام مشتری الزامی است")
            .MaximumLength(100)
            .WithMessage("نام مشتری نباید بیش از 100 کاراکتر باشد");
        
        RuleFor(x => x.CustomerPhone)
            .NotEmpty()
            .WithMessage("شماره تماس الزامی است")
            .Matches(@"^09\d{9}$")
            .WithMessage("شماره تماس باید با 09 شروع شود و 11 رقم باشد");
        
        RuleFor(x => x.CustomerEmail)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.CustomerEmail))
            .WithMessage("فرمت ایمیل صحیح نیست");
        
        RuleFor(x => x.ReservationDate)
            .NotEmpty()
            .WithMessage("تاریخ رزرو الزامی است")
            .Must(date => date.Date >= DateTime.UtcNow.Date)
            .WithMessage("تاریخ رزرو باید امروز یا آینده باشد");
        
        RuleFor(x => x.ReservationTime)
            .NotEmpty()
            .WithMessage("ساعت رزرو الزامی است");
        
        RuleFor(x => x.GuestsCount)
            .GreaterThan(0)
            .WithMessage("تعداد نفرات باید بیشتر از صفر باشد")
            .LessThanOrEqualTo(20)
            .WithMessage("تعداد نفرات نباید بیشتر از 20 نفر باشد");
        
        RuleFor(x => x.SpecialRequests)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.SpecialRequests))
            .WithMessage("درخواست‌های ویژه نباید بیش از 500 کاراکتر باشد");
        
        // Business Rule: Check reservation datetime is not in the past
        RuleFor(x => x)
            .Must(cmd => {
                var reservationDateTime = cmd.ReservationDate.Date.Add(cmd.ReservationTime);
                return reservationDateTime > DateTime.UtcNow;
            })
            .WithMessage("زمان رزرو باید در آینده باشد");
    }
}

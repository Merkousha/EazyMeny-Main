using FluentValidation;

namespace EazyMenu.Application.Features.Restaurants.Commands.UpdateRestaurant;

/// <summary>
/// Validator برای UpdateRestaurantCommand
/// </summary>
public class UpdateRestaurantCommandValidator : AbstractValidator<UpdateRestaurantCommand>
{
    public UpdateRestaurantCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("شناسه رستوران الزامی است");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام رستوران الزامی است")
            .MinimumLength(3).WithMessage("نام رستوران باید حداقل 3 کاراکتر باشد")
            .MaximumLength(200).WithMessage("نام رستوران نباید بیش از 200 کاراکتر باشد");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("توضیحات نباید بیش از 1000 کاراکتر باشد")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("آدرس الزامی است")
            .MaximumLength(500).WithMessage("آدرس نباید بیش از 500 کاراکتر باشد");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("شماره تلفن الزامی است")
            .Matches(@"^0\d{10}$").WithMessage("شماره تلفن باید 11 رقم و با 0 شروع شود");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("ایمیل الزامی است")
            .EmailAddress().WithMessage("فرمت ایمیل صحیح نیست");

        RuleFor(x => x.DeliveryFee)
            .GreaterThanOrEqualTo(0).WithMessage("هزینه ارسال نمی‌تواند منفی باشد");

        RuleFor(x => x.MinimumOrderAmount)
            .GreaterThanOrEqualTo(0).WithMessage("حداقل سفارش نمی‌تواند منفی باشد");
    }
}

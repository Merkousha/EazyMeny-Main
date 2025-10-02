using FluentValidation;

namespace EazyMenu.Application.Features.Reservations.Commands.CancelReservation;

/// <summary>
/// Validator برای CancelReservationCommand
/// </summary>
public class CancelReservationCommandValidator : AbstractValidator<CancelReservationCommand>
{
    public CancelReservationCommandValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty()
            .WithMessage("شناسه رزرو الزامی است");
        
        RuleFor(x => x.CancellationReason)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.CancellationReason))
            .WithMessage("دلیل لغو نباید بیش از 500 کاراکتر باشد");
    }
}
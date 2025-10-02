using FluentValidation;

namespace EazyMenu.Application.Features.Reservations.Commands.ConfirmReservation;

/// <summary>
/// Validator برای ConfirmReservationCommand
/// </summary>
public class ConfirmReservationCommandValidator : AbstractValidator<ConfirmReservationCommand>
{
    public ConfirmReservationCommandValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty()
            .WithMessage("شناسه رزرو الزامی است");
        
        RuleFor(x => x.TableNumber)
            .GreaterThan(0)
            .When(x => x.TableNumber.HasValue)
            .WithMessage("شماره میز باید بیشتر از صفر باشد");
    }
}
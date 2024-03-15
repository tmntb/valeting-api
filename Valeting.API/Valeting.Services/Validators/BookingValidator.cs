using FluentValidation;
using Valeting.Services.Objects.Booking;

namespace Valeting.Services.Validators;

public class CreateBookingValidator : AbstractValidator<CreateBookingSVRequest>
{
    public CreateBookingValidator()
    {
        RuleFor(x => x)
                .NotNull();

        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.ContactNumber)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.BookingDate)
            .NotEqual(DateTime.MinValue)
            .LessThan(DateTime.Now);
    }
}
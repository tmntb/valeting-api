using FluentValidation;
using Service.Models.Booking;

namespace Service.Validators;

public class UpdateBookingValidator : AbstractValidator<BookingDto>
{
    public UpdateBookingValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty);

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
            .GreaterThan(DateTime.Now);

        RuleFor(x => x.Flexibility.Id)
           .NotEqual(Guid.Empty)
           .When(x => x.Flexibility != null);

        RuleFor(x => x.VehicleSize.Id)
            .NotEqual(Guid.Empty)
            .When(x => x.VehicleSize != null);
    }
}

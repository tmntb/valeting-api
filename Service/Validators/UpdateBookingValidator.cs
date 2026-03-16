using FluentValidation;
using Service.Models.Booking;

namespace Service.Validators;

public class UpdateBookingValidator : AbstractValidator<BookingDto>
{
    public UpdateBookingValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.Flexibility.Id)
           .NotEqual(Guid.Empty)
           .When(x => x.Flexibility != null);

        RuleFor(x => x.VehicleSize.Id)
            .NotEqual(Guid.Empty)
            .When(x => x.VehicleSize != null);

        RuleFor(x => x.ScheduledAt)
            .NotEqual(DateTime.MinValue)
            .GreaterThan(DateTime.Now);

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}

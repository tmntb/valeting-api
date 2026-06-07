using FluentValidation;
using Service.Models.Booking;

namespace Service.Validators;

/// <summary>
/// Validator for creating a new booking. This class defines the validation rules for the properties of the <see cref="BookingDto"/> when creating a new booking record. It ensures that required fields are provided and that they meet specific criteria, such as valid GUIDs for related entities, future dates for scheduling, and maximum length for notes.
/// </summary>
public class CreateBookingValidator : AbstractValidator<BookingDto>
{
    public CreateBookingValidator()
    {
        RuleFor(x => x.Customer.Id)
            .NotEqual(Guid.Empty)
            .When(x => x.Customer != null);

        RuleFor(x => x.Flexibility.Id)
            .NotEqual(Guid.Empty)
            .When(x => x.Flexibility != null);

        RuleFor(x => x.VehicleSize.Id)
            .NotEqual(Guid.Empty)
            .When(x => x.VehicleSize != null);

        RuleFor(x => x.ScheduledAt)
            .NotEqual(DateTime.MinValue)
            .GreaterThan(DateTime.UtcNow);

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}

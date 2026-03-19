using FluentValidation;
using Service.Models.Booking;

namespace Service.Validators;

/// <summary>
/// Validator for updating booking requests. This class defines the validation rules for the properties of the <see cref="BookingDto"/> when updating an existing booking. It ensures that the Id is a valid GUID, the ScheduledAt date is in the future, and that any optional properties such as Flexibility and VehicleSize, if provided, have valid GUIDs. Additionally, it validates that the Notes property does not exceed a specified maximum length. This validation is crucial for maintaining data integrity and ensuring that updates to bookings adhere to business rules and constraints.
/// </summary>
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

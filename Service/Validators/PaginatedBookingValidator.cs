using System.Data;
using FluentValidation;
using Service.Models.Booking.Payload;

namespace Service.Validators;

/// <summary>
/// Validator for paginated booking requests. This class defines the validation rules for the properties of the <see cref="BookingFilterDto"/> when retrieving paginated booking data. It ensures that the Status, if provided, is a valid enumeration value, which is important for correctly filtering bookings based on their status.
/// </summary>
public class PaginatedBookingValidator : BaseFilterValidator<BookingFilterDto>
{
    public PaginatedBookingValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x != null && x.Status.HasValue);
    }
}
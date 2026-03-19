using FluentValidation;
using Service.Models.Booking.Payload;

namespace Service.Validators;

/// <summary>
/// Validator for paginated booking customer requests. This class defines the validation rules for the properties of the <see cref="BookingFilterDto"/> when retrieving paginated booking data for a specific customer. It ensures that the CustomerId, if provided, is a valid GUID and not an empty value, which is crucial for correctly filtering bookings based on the associated customer.
/// </summary>
public class PaginatedBookingCustomerValidator : BaseFilterValidator<BookingFilterDto>
{
    public PaginatedBookingCustomerValidator()
    {
        RuleFor(x => x.CustomerId)
            .Must(id => id.Value != Guid.Empty)
            .When(x => x.CustomerId != null);
    }
}

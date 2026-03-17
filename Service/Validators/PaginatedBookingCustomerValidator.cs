using FluentValidation;
using Service.Models.Booking.Payload;

namespace Service.Validators;

public class PaginatedBookingCustomerValidator : BaseFilterValidator<BookingFilterDto>
{
    public PaginatedBookingCustomerValidator()
    {
        RuleFor(x => x.CustomerId)
            .Must(id => id.Value != Guid.Empty)
            .When(x => x.CustomerId != null);
    }
}

using FluentValidation;
using Service.Models.Booking.Payload;

namespace Service.Validators;

public class PaginatedBookingCustomerValidator : BaseFilterValidator<BookingFilterDto>
{
    public PaginatedBookingCustomerValidator()
    {
        RuleFor(x => x)
            .NotNull();

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .When(x => x.CustomerId != null);
    }
}

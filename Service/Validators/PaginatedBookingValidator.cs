using FluentValidation;
using Service.Models.Booking.Payload;

namespace Service.Validators;

public class PaginatedBookingValidator : AbstractValidator<BookingFilterDto>
{
    public PaginatedBookingValidator()
    {
        RuleFor(x => x)
            .NotNull();

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .When(x => x != null);
    }
}
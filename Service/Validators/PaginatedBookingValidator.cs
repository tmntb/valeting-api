using System.Data;
using FluentValidation;
using Service.Models.Booking.Payload;

namespace Service.Validators;

public class PaginatedBookingValidator : BaseFilterValidator<BookingFilterDto>
{
    public PaginatedBookingValidator()
    {
        RuleFor(x => x)
            .NotNull();

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x != null && x.Status.HasValue);
    }
}
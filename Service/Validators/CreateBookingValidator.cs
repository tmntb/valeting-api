using FluentValidation;
using Service.Models.Booking;

namespace Service.Validators
{
    public class CreateBookingValidator : AbstractValidator<BookingDto>
    {
        public CreateBookingValidator()
        {
            RuleFor(x => x.Name)
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
}

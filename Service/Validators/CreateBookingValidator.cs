using FluentValidation;
using Service.Models.Booking;

namespace Service.Validators
{
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
                .GreaterThan(DateTime.Now);

            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .When(x => !string.IsNullOrEmpty(x.Notes));
        }
    }
}

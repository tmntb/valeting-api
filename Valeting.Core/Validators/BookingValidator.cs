using FluentValidation;
using Valeting.Common.Models.Booking;

namespace Valeting.Core.Validators;

public class CreateBookingValidator : AbstractValidator<CreateBookingDtoRequest>
{
    public CreateBookingValidator()
    {
        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.ContactNumber)
            .NotNull();

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

public class UpdateBookinValidator : AbstractValidator<UpdateBookingDtoRequest>
{
    public UpdateBookinValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.ContactNumber)
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

public class DeleteBookingValidator : AbstractValidator<DeleteBookingDtoRequest>
{
    public DeleteBookingValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty);
    }
}

public class GetBookingValidator : AbstractValidator<GetBookingDtoRequest>
{
    public GetBookingValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty);
    }
}

public class PaginatedBookingValidator : AbstractValidator<PaginatedBookingDtoRequest>
{
    public PaginatedBookingValidator()
    {
        RuleFor(x => x.Filter)
            .NotNull();

        RuleFor(x => x.Filter.PageNumber)
            .GreaterThan(0)
            .When(x => x.Filter != null);
    }
}
using FluentValidation;

using Valeting.Core.Models.Booking;
using Valeting.Core.Validators.Helper;

namespace Valeting.Core.Validators;

public class CreateBookingValidator : AbstractValidator<CreateBookingSVRequest>
{
    private readonly ValidationHelpers _validationHelpers;

    public CreateBookingValidator(ValidationHelpers validationHelpers)
    {
        _validationHelpers = validationHelpers;

        RuleFor(x => x)
            .NotNull();

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
            .MustAsync(_validationHelpers.FlexibilityIsValid);

        RuleFor(x => x.VehicleSize.Id)
            .NotEqual(Guid.Empty)
            .MustAsync(_validationHelpers.VehicleSizeIsValid);
    }
}

public class UpdateBookinValidator : AbstractValidator<UpdateBookingSVRequest>
{
    public UpdateBookinValidator()
    {
        RuleFor(x => x)
            .NotNull();

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
           .NotEqual(Guid.Empty);

        RuleFor(x => x.VehicleSize.Id)
            .NotEqual(Guid.Empty);
    }
}

public class DeleteBookingValidator : AbstractValidator<DeleteBookingSVRequest>
{
    public DeleteBookingValidator()
    {
        RuleFor(x => x)
            .NotNull();

        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty);
    }
}

public class GetBookingValidator : AbstractValidator<GetBookingSVRequest>
{
    public GetBookingValidator()
    {
        RuleFor(x => x)
            .NotNull();

        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty);
    }
}

public class PaginatedBookingValidator : AbstractValidator<PaginatedBookingSVRequest>
{
    public PaginatedBookingValidator()
    {
        RuleFor(x => x)
            .NotNull();

        RuleFor(x => x.Filter)
            .NotNull();

        RuleFor(x => x.Filter.PageNumber)
            .GreaterThanOrEqualTo(0);
    }
}
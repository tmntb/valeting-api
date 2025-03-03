using FluentValidation;
using Valeting.Common.Models.VehicleSize;

namespace Valeting.Services.Validators;

public class GetVehicleSizeValidator : AbstractValidator<GetVehicleSizeDtoRequest>
{
    public GetVehicleSizeValidator()
    {
        RuleFor(x => x)
            .NotNull();

        RuleFor(x => x.Id)
            .NotNull()
            .NotEqual(Guid.Empty);
    }
}

public class PaginatedVehicleSizeValidator : AbstractValidator<PaginatedVehicleSizeDtoRequest>
{
    public PaginatedVehicleSizeValidator()
    {
        RuleFor(x => x)
            .NotNull();

        RuleFor(x => x.Filter)
            .NotNull();

        RuleFor(x => x.Filter.PageNumber)
            .GreaterThanOrEqualTo(0);
    }
}
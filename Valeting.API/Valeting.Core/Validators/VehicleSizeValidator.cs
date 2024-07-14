using FluentValidation;

using Valeting.Core.Models.VehicleSize;

namespace Valeting.Core.Validators;

public class GetVehicleSizeValidator : AbstractValidator<GetVehicleSizeSVRequest>
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

public class PaginatedVehicleSizeValidator : AbstractValidator<PaginatedVehicleSizeSVRequest>
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
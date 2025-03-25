using FluentValidation;
using Valeting.Common.Models.VehicleSize;

namespace Valeting.Core.Validators;

public class GetVehicleSizeValidator : AbstractValidator<GetVehicleSizeDtoRequest>
{
    public GetVehicleSizeValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
            .NotEqual(Guid.Empty);
    }
}

public class PaginatedVehicleSizeValidator : AbstractValidator<PaginatedVehicleSizeDtoRequest>
{
    public PaginatedVehicleSizeValidator()
    {
        RuleFor(x => x.Filter)
            .NotNull();

        RuleFor(x => x.Filter.PageNumber)
            .GreaterThan(0)
            .When(x => x.Filter != null);
    }
}
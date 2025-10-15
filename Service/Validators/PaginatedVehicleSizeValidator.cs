using FluentValidation;
using Service.Models.VehicleSize.Payload;

namespace Service.Validators;

public class PaginatedVehicleSizeValidator : AbstractValidator<VehicleSizeFilterDto>
{
    public PaginatedVehicleSizeValidator()
    {
        RuleFor(x => x)
            .NotNull();

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .When(x => x != null);
    }
}
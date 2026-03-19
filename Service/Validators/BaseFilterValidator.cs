using FluentValidation;
using Service.Models.Core;

namespace Service.Validators;

/// <summary>
/// Base validator for filter DTOs that include pagination.
/// Validates common pagination properties.
/// </summary>
public class BaseFilterValidator<T> : AbstractValidator<T> where T : FilterDto
{
    public BaseFilterValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page Number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page Size must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Page Size cannot exceed 100.");
    }
}
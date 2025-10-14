using FluentValidation;
using Common.Models.Flexibility;

namespace Service.Validators;

public class PaginatedFlexibilityValidator : AbstractValidator<FlexibilityFilterDto>
{
    public PaginatedFlexibilityValidator()
    {
        RuleFor(x => x)
            .NotNull();

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .When(x => x != null);
    }
}
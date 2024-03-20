using FluentValidation;
using Valeting.Services.Objects.Flexibility;

namespace Valeting.Services.Validators;

public class GetFlexibilityValidator : AbstractValidator<GetFlexibilitySVRequest>
{
    public GetFlexibilityValidator()
    {
        RuleFor(x => x)
            .NotNull();

        RuleFor(x => x.Id)
            .NotNull()
            .NotEqual(Guid.Empty);
    }
}

public class PaginatedFlexibilityValidator : AbstractValidator<PaginatedFlexibilitySVRequest>
{
    public PaginatedFlexibilityValidator()
    {
        RuleFor(x => x)
            .NotNull();

        RuleFor(x => x.Filter)
            .NotNull();

        RuleFor(x => x.Filter.PageNumber)
            .GreaterThanOrEqualTo(0);
    }
}
using FluentValidation;
using Valeting.Common.Models.Flexibility;

namespace Valeting.Core.Validators;

public class GetFlexibilityValidator : AbstractValidator<GetFlexibilityDtoRequest>
{
    public GetFlexibilityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
            .NotEqual(Guid.Empty);
    }
}

public class PaginatedFlexibilityValidator : AbstractValidator<PaginatedFlexibilityDtoRequest>
{
    public PaginatedFlexibilityValidator()
    {
        RuleFor(x => x.Filter)
            .NotNull();

        RuleFor(x => x.Filter.PageNumber)
            .GreaterThan(0)
            .When(x => x.Filter != null);
    }
}
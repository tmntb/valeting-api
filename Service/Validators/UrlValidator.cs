using FluentValidation;
using Common.Models.Link;

namespace Service.Validators;

public class GenerateSelfUrlValidator : AbstractValidator<GenerateSelfUrlDtoRequest>
{
    public GenerateSelfUrlValidator()
    {
        RuleFor(x => x.Request)
           .NotNull();
    }
}

public class GeneratePaginatedLinksValidator : AbstractValidator<GeneratePaginatedLinksDtoRequest>
{
    public GeneratePaginatedLinksValidator()
    {
        RuleFor(x => x.Request)
           .NotNull();
    }
}

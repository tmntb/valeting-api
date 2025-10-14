using FluentValidation;
using Common.Models.Link;

namespace Service.Validators;

public class GeneratePaginatedLinksValidator : AbstractValidator<GeneratePaginatedLinksDtoRequest>
{
    public GeneratePaginatedLinksValidator()
    {
        RuleFor(x => x.Request)
           .NotNull();
    }
}

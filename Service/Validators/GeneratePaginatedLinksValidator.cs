using FluentValidation;
using Service.Models.Link.Payload;

namespace Service.Validators;

public class GeneratePaginatedLinksValidator : AbstractValidator<GeneratePaginatedLinksDtoRequest>
{
    public GeneratePaginatedLinksValidator()
    {
        RuleFor(x => x.Request)
           .NotNull();
    }
}

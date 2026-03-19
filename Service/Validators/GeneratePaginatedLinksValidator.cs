using FluentValidation;
using Service.Models.Link.Payload;

namespace Service.Validators;

/// <summary>
/// Validator for generating paginated links. This class defines the validation rules for the properties of the <see cref="GeneratePaginatedLinksDtoRequest"/> when generating pagination links for API responses. It ensures that the request object is not null, which is essential for successfully creating pagination links based on the provided parameters.
/// </summary>
public class GeneratePaginatedLinksValidator : AbstractValidator<GeneratePaginatedLinksDtoRequest>
{
    public GeneratePaginatedLinksValidator()
    {
        RuleFor(x => x.Request)
           .NotNull();
    }
}

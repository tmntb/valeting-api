using FluentValidation;
using Service.Models.Link.Payload;

namespace Service.Validators;

/// <summary>
/// Validator for generating self links. This class defines the validation rules for the properties of the <see cref="GenerateSelfLinkDtoRequest"/> when generating self links for API responses. It ensures that the request object is not null, which is essential for successfully creating self links based on the provided parameters.
/// </summary>
public class GenerateSelfLinkValidator : AbstractValidator<GenerateSelfLinkDtoRequest>
{
    public GenerateSelfLinkValidator()
    {
        RuleFor(x => x.Request)
           .NotNull();
    }
}

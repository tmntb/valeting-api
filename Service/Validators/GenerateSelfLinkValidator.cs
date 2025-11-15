using FluentValidation;
using Service.Models.Link.Payload;

namespace Service.Validators;

public class GenerateSelfLinkValidator : AbstractValidator<GenerateSelfLinkDtoRequest>
{
    public GenerateSelfLinkValidator()
    {
        RuleFor(x => x.Request)
           .NotNull();
    }
}

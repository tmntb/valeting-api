using Common.Models.Link;
using FluentValidation;

namespace Service.Validators;

public class GenerateSelfUrlValidator : AbstractValidator<GenerateSelfUrlDtoRequest>
{
    public GenerateSelfUrlValidator()
    {
        RuleFor(x => x.Request)
           .NotNull();
    }
}

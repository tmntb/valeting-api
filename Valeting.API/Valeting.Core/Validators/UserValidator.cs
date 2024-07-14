using FluentValidation;

using Valeting.Core.Models.User;

namespace Valeting.Core.Validators;

public class ValidateLoginValidator : AbstractValidator<ValidateLoginSVRequest>
{
    public ValidateLoginValidator()
    {
        RuleFor(x => x)
            .NotNull();

        RuleFor(x => x.Username)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotNull()
            .NotEmpty();
    }
}

public class GenerateTokenJWTValidator : AbstractValidator<GenerateTokenJWTSVRequest>
{
    public GenerateTokenJWTValidator()
    {
        RuleFor(x => x)
            .NotNull();
    }
}
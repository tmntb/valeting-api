using FluentValidation;
using Valeting.Common.Models.User;

namespace Valeting.Core.Validators;

public class ValidateLoginValidator : AbstractValidator<ValidateLoginDtoRequest>
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

public class GenerateTokenJWTValidator : AbstractValidator<GenerateTokenJWTDtoRequest>
{
    public GenerateTokenJWTValidator()
    {
        RuleFor(x => x)
            .NotNull();
    }
}
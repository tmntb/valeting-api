using FluentValidation;
using Service.Models.Token.Payload;

namespace Service.Validators.Token;

/// <summary>
/// Validates JWT generation requests.
/// </summary>
public class GenerateTokenJwtValidator : AbstractValidator<GenerateTokenJwtDtoRequest>
{
    /// <summary>
    /// Initializes the email validation rule.
    /// </summary>
    public GenerateTokenJwtValidator()
    {
        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();
    }
}
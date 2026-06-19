using FluentValidation;
using Service.Models.Auth.Payload;

namespace Service.Validators.Auth;

/// <summary>
/// Validator to mfa code. Validates that the user id and mfa code are in a proper format.
/// </summary>
public class MfaCodeValidator : AbstractValidator<MfaCodeDtoRequest>
{
    public MfaCodeValidator()
    {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.MfaCode)
            .NotNull()
            .NotEmpty()
            .Length(6)
            .Matches(@"^\d+$");
    }
}

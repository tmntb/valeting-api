using FluentValidation;
using Service.Models.Auth.Payload;

namespace Service.Validators.Auth;

/// <summary>
/// Validator to enable mfa. Validates that the email and mfa code are in a proper format.
/// </summary>
public class MfaEnableValidator : AbstractValidator<MfaEnableDtoRequest>
{
    public MfaEnableValidator()
    {
        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.MfaCode)
            .NotNull()
            .NotEmpty()
            .Length(6)
            .Matches(@"^\d+$");
    }
}

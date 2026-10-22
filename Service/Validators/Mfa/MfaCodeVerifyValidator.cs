using FluentValidation;
using Service.Models.Auth.Payload;
using Service.Models.Mfa.Payload;

namespace Service.Validators.Auth;

/// <summary>
/// Validator to mfa code. Validates that the user id and mfa code are in a proper format.
/// </summary>
public class MfaCodeVerifyValidator : AbstractValidator<MfaCodeDtoRequest>
{
    public MfaCodeVerifyValidator()
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

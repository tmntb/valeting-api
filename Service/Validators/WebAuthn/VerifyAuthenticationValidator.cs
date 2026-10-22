using FluentValidation;
using Service.Models.WebAuthn.Payload;

namespace Service.Validators.WebAuthn;

/// <summary>
/// Defines validation rules for webauthn authentication verification requests.
/// </summary>
public class VerifyAuthenticationValidator : AbstractValidator<WebAuthnAuthenticationVerifyRequest>
{
    /// <summary>
    /// Initializes the validation rules for a webauthn authentication verification request.
    /// </summary>
    public VerifyAuthenticationValidator()
    {
        RuleFor(x => x.CeremonyId)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.Credential)
            .NotNull();
    }
}
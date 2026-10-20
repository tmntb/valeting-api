using FluentValidation;
using Service.Models.WebAuthn.Payload;

namespace Service.Validators.WebAuthn;

/// <summary>
/// Defines validation rules for webauthn registration verification requests.
/// </summary>
public class VerifyRegisterCredentialsValidator : AbstractValidator<WebAuthnRegistrationVerifyDtoRequest>
{
    /// <summary>
    /// Initializes the validation rules for a webauthn registration verification request.
    /// </summary>
    public VerifyRegisterCredentialsValidator()
    {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.CeremonyId)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.Credential)
            .NotNull();

        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty()
            .MaximumLength(200);
    }
}

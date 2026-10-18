using FluentValidation;
using Service.Models.Passkey.Payload;

namespace Service.Validators.Passkey;

/// <summary>
/// Defines validation rules for passkey registration verification requests.
/// </summary>
public class PasskeyRegistrationVerifyValidator : AbstractValidator<PasskeyRegistrationVerifyDtoRequest>
{
    /// <summary>
    /// Initializes the validation rules for a passkey registration verification request.
    /// </summary>
    public PasskeyRegistrationVerifyValidator()
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

using FluentValidation;
using Service.Models.User;

namespace Service.Validators.WebAuthn;

/// <summary>
/// Validator for webauthn authentication option requests.
/// This class defines the validation rules for the properties of the <see cref="UserDto"/> when a user attempts two factor auth.
/// It ensures that the Email is provided, not empty, and in a valid email format,
/// This validation is essential for maintaining data integrity and ensuring that login attempts are properly validated before processing authentication logic.
/// </summary>
public class CreateAuthenticationOptionsValidator : AbstractValidator<UserDto>
{
    public CreateAuthenticationOptionsValidator()
    {
        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();
    }
}
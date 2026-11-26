using FluentValidation;
using Service.Models.Auth.Payload;

namespace Service.Validators.Auth;

/// <summary>
/// Defines validation rules for forgot-password requests.
/// </summary>
public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordDtoRequest>
{
    /// <summary>
    /// Initializes the validator with rules for the account email.
    /// </summary>
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();
    }
}

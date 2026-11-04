using FluentValidation;
using Service.Models.Auth.Payload;

namespace Service.Validators.Auth;

/// <summary>
/// Defines validation rules for forgot-password requests.
/// </summary>
public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordDtoRequest>
{
    /// <summary>
    /// Initializes the validator with rules for the account email, recovery method, and new password.
    /// </summary>
    public ForgotPasswordValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.MfaCode) ^ !string.IsNullOrWhiteSpace(x.RecoveryCode))
            .WithMessage("Either MfaCode or RecoveryCode must be provided, but not both.");

        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.MfaCode)
            .Length(6)
            .Matches(@"^\d+$");

        RuleFor(x => x.RecoveryCode)
            .Matches(@"^\d{4}-\d{4}$")
            .When(x => x.RecoveryCode != null);

        RuleFor(x => x.NewPassword)
            .NotNull()
            .NotEmpty();
    }
}

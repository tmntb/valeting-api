using FluentValidation;
using Service.Models.RecoveryCode.Payload;

namespace Service.Validators.RecoveryCode;

/// <summary>
/// Defines validation rules for verify recovery code requests.
/// </summary>
public class RecoveryCodeVerifyValidator : AbstractValidator<VerifyRecoveryCodeDtoRequest>
{
    /// <summary>
    /// Initializes the validator with rules for the account email and recovery method.
    /// </summary>
    public RecoveryCodeVerifyValidator()
    {
        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Code)
            .NotNull()
            .NotEmpty()
            .Matches(@"^\d{4}-\d{4}$");
    }
}

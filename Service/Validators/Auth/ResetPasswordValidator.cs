using FluentValidation;
using Service.Models.Auth.Payload;

namespace Service.Validators.Auth;

/// <summary>
/// Validates password reset requests.
/// </summary>
public class ResetPasswordValidator : AbstractValidator<ResetPasswordDtoRequest>
{
    /// <summary>
    /// Initializes token and new-password validation rules.
    /// </summary>
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Token)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotNull()
            .NotEmpty();
    }
}

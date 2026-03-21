using FluentValidation;
using Service.Models.User;

namespace Service.Validators.User;

/// <summary>
/// Validator for resetting a user's password. Validates that the email is in a proper format and that the new password is not null or empty.
/// </summary>
public class ResetValidator : AbstractValidator<UserDto>
{
    
    public ResetValidator()
    {
        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotNull()
            .NotEmpty();
    }
}
using FluentValidation;
using Service.Models.User;

namespace Service.Validators.User;

/// <summary>
/// Defines validation rules for user email updates.
/// </summary>
public class UpdateEmailValidator : AbstractValidator<UserDto>
{
    /// <summary>
    /// Initializes the validator with rules requiring a non-empty user identifier and a valid email address.
    /// </summary>
    public UpdateEmailValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}

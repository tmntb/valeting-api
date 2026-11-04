using FluentValidation;
using Service.Models.User;

namespace Service.Validators.User;

/// <summary>
/// Defines validation rules for user password updates.
/// </summary>
public class UpdatePasswordValidator : AbstractValidator<UserDto>
{ 
    /// <summary>
    /// Initializes the validator with rules requiring a non-empty password.
    /// </summary>
    public UpdatePasswordValidator()
    {
        RuleFor(u => u.Password)
            .NotNull()
            .NotEmpty();
    }
}

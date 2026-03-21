using FluentValidation;
using Service.Models.User;
using Service.Models.User.Payload;

namespace Service.Validators.User;

/// <summary>
/// Validator for validating login requests. This class defines the validation rules for the properties of the <see cref="ValidateLoginDtoRequest"/> when a user attempts to log in. It ensures that the Email is provided, not empty, and in a valid email format, while also ensuring that the Password is provided and not empty. This validation is essential for maintaining data integrity and ensuring that login attempts are properly validated before processing authentication logic.
/// </summary>
public class ValidateLoginValidator : AbstractValidator<UserDto>
{
    public ValidateLoginValidator()
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

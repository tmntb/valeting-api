using FluentValidation;
using Service.Models.User.Payload;

namespace Service.Validators;

/// <summary>
/// Validator for user registration requests. This class defines the validation rules for the properties of the <see cref="RegisterDtoRequest"/> when a new user is registering. It ensures that required fields such as Username, Password, ContactNumber, Email, and RoleCode are provided and meet specific criteria, such as valid email format, non-empty values, and valid enumeration values for the role code. This validation is crucial for maintaining data integrity and ensuring that the registration process captures all necessary information for creating a new user account.
/// </summary>
public class RegisterValidator : AbstractValidator<RegisterDtoRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Username)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.ContactNumber)
            .Must(x => x.ToString().Length == 9);

        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.RoleCode)
            .IsInEnum();
    }
}
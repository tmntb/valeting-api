using FluentValidation;
using Service.Models.User;

namespace Service.Validators.User;

/// <summary>
/// Validator for user registration requests. This class defines the validation rules for the properties of the <see cref="UserDto"/> when a new user is registering. 
/// It ensures that required fields such as Username, Password, ContactNumber, Email, and Role Code are provided and meet specific criteria, such as valid email format, non-empty values, and valid enumeration values for the role code. 
/// This validation is crucial for maintaining data integrity and ensuring that the registration process captures all necessary information for creating a new user account.
/// </summary>
public class RegisterValidator : AbstractValidator<UserDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.FirstName)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.LastName)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.DateOfBirth)
            .NotEqual(DateOnly.MinValue);

        RuleFor(x => x.ContactNumber)
            .Must(x => x.ToString().Length == 9);

        RuleFor(x => x.Role.Code)
            .IsInEnum();
    }
}
using FluentValidation;
using Service.Models.User.Payload;

namespace Service.Validators;

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
            .Must(x => x.ToString().Length == 9)
            .NotNull();

        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.RoleName)
            .IsInEnum();
    }
}
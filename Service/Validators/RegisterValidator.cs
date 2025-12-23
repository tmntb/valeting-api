using FluentValidation;
using Service.Models.User.Payload;

namespace Service.Validators;

public class RegisterValidator : AbstractValidator<RegisterDtoRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Username)
            .NotNull()
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotNull()
            .NotEmpty();
    }
}
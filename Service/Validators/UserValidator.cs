using FluentValidation;
using Common.Models.User;

namespace Service.Validators;

public class ValidateLoginValidator : AbstractValidator<ValidateLoginDtoRequest>
{
    public ValidateLoginValidator()
    {
        RuleFor(x => x.Username)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotNull()
            .NotEmpty();
    }
}

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
    }
}
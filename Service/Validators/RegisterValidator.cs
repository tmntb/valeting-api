using FluentValidation;
using Common.Models.User;

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
    }
}
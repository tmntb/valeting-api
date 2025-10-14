using Common.Models.User;
using FluentValidation;

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

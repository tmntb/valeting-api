using FluentValidation;
using Service.Models.User.Payload;

namespace Service.Validators;

public class ValidateLoginValidator : AbstractValidator<ValidateLoginDtoRequest>
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

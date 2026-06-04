using FluentValidation;
using Service.Models.User;

namespace Service.Validators.User;

public class UpdateEmailValidator : AbstractValidator<UserDto>
{
    public UpdateEmailValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}

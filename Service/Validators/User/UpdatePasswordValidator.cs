using FluentValidation;
using Service.Models.User;

namespace Service.Validators.User;

public class UpdatePasswordValidator : AbstractValidator<UserDto>
{ 
    public UpdatePasswordValidator()
    {
        RuleFor(u => u.Password)
            .NotNull()
            .NotEmpty();
    }
}

using FluentValidation;
using Service.Models.User.Payload;

namespace Service.Validators.User;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileDtoRequest>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.FirstName)
             .NotEmpty()
             .MaximumLength(50)
             .When(x => x.FirstName != null);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50)
            .When(x => x.LastName != null);

        RuleFor(x => x.DateOfBirth)
            .NotEqual(DateOnly.MinValue)
            .When(x => x.DateOfBirth != null);

        RuleFor(x => x.ContactNumber)
            .Must(x => x.ToString().Length == 9)
            .When(x => x.ContactNumber != null);
    }
}

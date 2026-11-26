using FluentValidation;
using Service.Models.Email;

namespace Service.Validators.Email;

/// <summary>
/// Validates outbound email messages.
/// </summary>
public class SendEmailValidator : AbstractValidator<EmailDto>
{
    /// <summary>
    /// Initializes recipient, subject, and body validation rules.
    /// </summary>
    public SendEmailValidator()
    {
        RuleFor(x => x.To)
            .NotNull()
            .NotEmpty();
        
        RuleFor(x => x.Subject)
            .NotNull()
            .NotEmpty();
        
        RuleFor(x => x.Body)
            .NotNull()
            .NotEmpty();
    }
}
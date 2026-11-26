using Resend;
using Service.Interfaces;
using Service.Models.Email;
using Service.Validators.Email;
using Service.Validators.Utils;

namespace Service.Services;

public sealed class EmailService(IResend resend) : IEmailService
{
    /// <inheritdoc />
    public async Task SendAsync(EmailDto emailDto, CancellationToken cancellationToken = default)
    {
        emailDto.ValidateRequest(new SendEmailValidator());

        var message = new EmailMessage
        {
            From = "NoReply <onboarding@resend.dev>"
        };

        message.To.Add(emailDto.To);
        message.Subject = emailDto.Subject;
        message.HtmlBody = emailDto.Body;

        await resend.EmailSendAsync(message, cancellationToken);
    }
}
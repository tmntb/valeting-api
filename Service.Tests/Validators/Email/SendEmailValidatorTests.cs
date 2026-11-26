using Service.Models.Email;
using Service.Validators.Email;

namespace Service.Tests.Validators.Email;

public class SendEmailValidatorTests
{
    private readonly SendEmailValidator _validator = new();

    [Theory]
    [InlineData("", "Subject", "Body")]
    [InlineData("recipient@example.com", "", "Body")]
    [InlineData("recipient@example.com", "Subject", "")]
    [InlineData("recipient@example.com", "Subject", null)]
    public void InvalidMessage_ShouldFail(string to, string subject, string? body)
    {
        // Act
        var result = _validator.Validate(new EmailDto
        {
            To = to,
            Subject = subject,
            Body = body!
        });
        
        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void ValidMessage_ShouldPass()
    {
        // Act
        var result = _validator.Validate(new EmailDto
        {
            To = "recipient@example.com",
            Subject = "Subject",
            Body = "Body"
        });

        // Assert
        Assert.True(result.IsValid);
    }
}

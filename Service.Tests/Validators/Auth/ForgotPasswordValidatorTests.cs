using Service.Models.Auth.Payload;
using Service.Validators.Auth;

namespace Service.Tests.Validators.Auth;

public class ForgotPasswordValidatorTests
{
    private readonly ForgotPasswordValidator _validator;

    public ForgotPasswordValidatorTests()
    {
        _validator = new ForgotPasswordValidator(); 
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid-email")]
    public void Email_ShouldFail(string? email)
    {
        // Arrange
        var request = new ForgotPasswordDtoRequest
        {
            Email = email
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Email", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void ValidEmail_ShouldPass()
    {
        var request = new ForgotPasswordDtoRequest
        {
            Email = "user@example.com"
        };
        var result = _validator.Validate(request);
        Assert.True(result.IsValid);
    }
}

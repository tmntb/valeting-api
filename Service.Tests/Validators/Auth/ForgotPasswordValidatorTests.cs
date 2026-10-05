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

    [Fact]
    public void BothMfaAndRecoveryCode_ShouldFail()
    {
        // Arrange
        var request = new ForgotPasswordDtoRequest
        {
            Email = "user@example.com",
            MfaCode = "123456",
            RecoveryCode = "1234-5678"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Either MfaCode or RecoveryCode must be provided, but not both.", result.Errors.FirstOrDefault().ErrorMessage);
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
            Email = email,
            MfaCode = "123456"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Email", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Theory]
    [InlineData("invalid-mfa-code")]
    public void MfaCode_ShouldFail(string? mfaCode)
    {
        // Arrange
        var request = new ForgotPasswordDtoRequest
        {
            Email = "user@example.com",
            MfaCode = mfaCode
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Mfa Code", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Theory]
    [InlineData("invalid-recovery-code")]
    public void RecoveryCode_ShouldFail(string? recoveryCode)
    {
        // Arrange
        var request = new ForgotPasswordDtoRequest
        {
            Email = "user@example.com",
            RecoveryCode = recoveryCode
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Recovery Code", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NewPassword_ShouldFail(string? newPassword)
    {
        // Arrange
        var request = new ForgotPasswordDtoRequest
        {
            Email = "user@example.com",
            MfaCode = "123456",
            NewPassword = newPassword
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("New Password", result.Errors.FirstOrDefault().ErrorMessage);
    }
}

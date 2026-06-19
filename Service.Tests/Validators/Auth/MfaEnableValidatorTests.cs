using System;
using Service.Models.Auth.Payload;
using Service.Validators.Auth;

namespace Service.Tests.Validators.Auth;

public class MfaEnableValidatorTests
{
    private readonly MfaEnableValidator _validator;

    public MfaEnableValidatorTests()
    {
        _validator = new MfaEnableValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid-email")]
    public void Email_ShouldFail(string? email)
    {
        // Arrange
        var request = new MfaEnableDtoRequest
        {
            Email = email
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Email", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid-mfa-code")]
    public void MfaCode_ShouldFail(string? mfaCode)
    {
        // Arrange
        var request = new MfaEnableDtoRequest
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
}

using Service.Models.Auth.Payload;
using Service.Models.Mfa.Payload;
using Service.Validators.Auth;

namespace Service.Tests.Validators.Auth;

public class MfaCodeVerifyValidatorTests
{
    private readonly MfaCodeVerifyValidator _verifyValidator;

    public MfaCodeVerifyValidatorTests()
    {
        _verifyValidator = new MfaCodeVerifyValidator();
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public void UserId_ShouldFail(Guid userId)
    {
        // Arrange
        var request = new MfaCodeDtoRequest
        {
            UserId = userId
        };

        // Act
        var result = _verifyValidator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("User Id", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid-mfa-code")]
    public void MfaCode_ShouldFail(string? mfaCode)
    {
        // Arrange
        var request = new MfaCodeDtoRequest
        {
            UserId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            MfaCode = mfaCode
        };

        // Act
        var result = _verifyValidator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Mfa Code", result.Errors.FirstOrDefault().ErrorMessage);
    }
}

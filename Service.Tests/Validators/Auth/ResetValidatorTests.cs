using Service.Models.User.Payload;
using Service.Validators.User;

namespace Service.Tests.Validators.Auth;

public class ResetValidatorTests
{
    private readonly ResetValidator _validator;

    public ResetValidatorTests()
    {
        _validator = new ResetValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Code_ShouldFail(string? code)
    {
        // Arrange
        var request = new ResetPasswordDtoRequest
        {
            Code = code
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Code", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Email_ShouldFail(string? email)
    {
        // Arrange
        var request = new ResetPasswordDtoRequest
        {
            Code = "code",
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
    public void Password_ShouldFail(string? password)
    {
        // Arrange
        var request = new ResetPasswordDtoRequest
        {
            Code = "code",
            Email = "user@example.com",
            NewPassword = password
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Password", result.Errors.FirstOrDefault().ErrorMessage);
    }
}

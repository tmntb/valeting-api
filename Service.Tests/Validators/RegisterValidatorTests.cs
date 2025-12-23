using Service.Models.User.Payload;
using Service.Validators;

namespace Service.Tests.Validators;

public class RegisterValidatorTests
{
    private readonly RegisterValidator _validator;

    public RegisterValidatorTests()
    {
        _validator = new RegisterValidator();
    }

    [Fact]
    public void Username_Null_ShouldFail()
    {
        // Arrange
        var request = new RegisterDtoRequest
        {
            Username = null,
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Username", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void Username_Empty_ShouldFail()
    {
        // Arrange
        var request = new RegisterDtoRequest
        {
            Username = string.Empty,
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Username", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void Password_Null_ShouldFail()
    {
        // Arrange
        var request = new RegisterDtoRequest
        {
            Username = "username@username.com",
            Password = null
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Password", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void Password_Empty_ShouldFail()
    {
        // Arrange
        var request = new RegisterDtoRequest
        {
            Username = "username@username.com",
            Password = string.Empty
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Password", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void RegisterDtoRequest_Valid()
    {
        // Arrange
        var request = new RegisterDtoRequest
        {
            Username = "username@username.com",
            Password = "password"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }
}

using Service.Models.User.Payload;
using Service.Validators;

namespace Service.Tests.Validators;

public class ValidateLoginValidatorTests
{
    private readonly ValidateLoginValidator _validator;

    public ValidateLoginValidatorTests()
    {
        _validator = new ValidateLoginValidator();
    }

    [Fact]
    public void Email_Null_ShouldFail() 
    {
        // Arrange
        var request = new ValidateLoginDtoRequest
        {
            Email = null,
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Email", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void Email_Empty_ShouldFail() 
    {
        // Arrange
        var request = new ValidateLoginDtoRequest
        {
            Email = string.Empty,
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Email", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void Password_Null_ShouldFail() 
    {
        // Arrange
        var request = new ValidateLoginDtoRequest
        {
            Email = "user@example.com",
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
        var request = new ValidateLoginDtoRequest
        {
            Email = "user@example.com",
            Password = string.Empty
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Password", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void ValidateLoginDtoRequest_Valid()
    {
        // Arrange
        var request = new ValidateLoginDtoRequest
        {
            Email = "user@example.com",
            Password = "password"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }
}

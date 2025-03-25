using Valeting.Common.Models.User;
using Valeting.Core.Validators;

namespace Valeting.Tests.Core.Validators;

#region ValidateLoginValidatorTests
public class ValidateLoginValidatorTests
{
    private readonly ValidateLoginValidator _validator;

    public ValidateLoginValidatorTests()
    {
        _validator = new ValidateLoginValidator();
    }

    [Fact]
    public void Username_Null_ShouldFail() 
    {
        // Arrange
        var request = new ValidateLoginDtoRequest
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
        var request = new ValidateLoginDtoRequest
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
        var request = new ValidateLoginDtoRequest
        {
            Username = "username",
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
            Username = "username",
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
            Username = "username",
            Password = "password"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }
}
#endregion

#region RegisterValidatorTests
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
            Username = "username",
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
            Username = "username",
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
            Username = "username",
            Password = "password"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }
}
#endregion
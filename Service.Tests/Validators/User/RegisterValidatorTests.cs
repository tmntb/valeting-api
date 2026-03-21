using Common.Enums;
using Service.Models.User;
using Service.Validators.User;

namespace Service.Tests.Validators.User;

public class RegisterValidatorTests
{
    private readonly RegisterValidator _validator;

    public RegisterValidatorTests()
    {
        _validator = new RegisterValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Username_ShouldFail(string? username)
    {
        // Arrange
        var request = new UserDto
        {
            Email = username
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Username", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Password_ShouldFail(string? password)
    {
        // Arrange
        var request = new UserDto
        {
            Username = "username",
            Password = password
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Password", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void ContactNumber_LengthNot9_ShouldFail()
    {
        // Arrange
        var request = new UserDto
        {
            Username = "username",
            Password = "password",
            ContactNumber = 12345678
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Contact Number", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid-email")]
    public void Email_ShouldFail(string? email)
    {
        // Arrange
        var request = new UserDto
        {
            Username = "username",
            Password = "password",
            ContactNumber = 123456789,
            Email = email
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Email", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void RoleCode_IsDefault_ShouldFail()
    {
        // Arrange
        var request = new UserDto
        {
            Username = "username",
            Password = "password",
            ContactNumber = 123456789,
            Email = "username@username.com",
            Role = new()
            {
                Code = (RoleEnum)999
            }
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Role Code", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void RegisterDtoRequest_Valid()
    {
        // Arrange
        var request = new UserDto
        {
            Username = "username",
            Password = "password",
            ContactNumber = 123456789,
            Email = "username@username.com",
             Role = new()
            {
                Code = RoleEnum.USER
            }
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }
}

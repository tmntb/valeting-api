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
    [InlineData("invalid-email")]
    public void Email_ShouldFail(string? email)
    {
        // Arrange
        var request = new UserDto
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
    public void Password_ShouldFail(string? password)
    {
        // Arrange
        var request = new UserDto
        {
            Email = "user@example.com",
            Password = password
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Password", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void FirstName_ShouldFail(string? firstName)
    {
        // Arrange
        var request = new UserDto
        {
            Email = "user@example.com",
            Password = "password",
            FirstName = firstName
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("First Name", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void LastName_ShouldFail(string? lastName)
    {
        // Arrange
        var request = new UserDto
        {
            Email = "user@example.com",
            Password = "password",
            FirstName = "firstName",
            LastName = lastName
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Last Name", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void DateOfBirth_ShouldFail()
    {
        // Arrange
        var request = new UserDto
        {
            Email = "user@example.com",
            Password = "password",
            FirstName = "firstName",
            LastName = "lastName",
            DateOfBirth = default
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Date Of Birth", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void ContactNumber_LengthNot9_ShouldFail()
    {
        // Arrange
        var request = new UserDto
        {
            Email = "user@example.com",
            Password = "password",
            FirstName = "firstName",
            LastName = "lastName",
            DateOfBirth = new DateOnly(1956, 10, 25),
            ContactNumber = 12345678
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Contact Number", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void RoleCode_IsDefault_ShouldFail()
    {
        // Arrange
        var request = new UserDto
        {
            Email = "username@username.com",
            Password = "password",
            FirstName = "firstName",
            LastName = "lastName",
            DateOfBirth = new DateOnly(1956, 10, 25),
            ContactNumber = 123456789,
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
            Email = "username@username.com",
            Password = "password",
            FirstName = "firstName",
            LastName = "lastName",
            DateOfBirth = new DateOnly(1956, 10, 25),
            ContactNumber = 123456789,
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

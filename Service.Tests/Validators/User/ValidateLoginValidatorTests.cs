using Service.Models.User;
using Service.Validators.User;

namespace Service.Tests.Validators.User;

public class ValidateLoginValidatorTests
{
    private readonly ValidateLoginValidator _validator;

    public ValidateLoginValidatorTests()
    {
        _validator = new ValidateLoginValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
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

    [Fact]
    public void ValidateLoginDtoRequest_Valid()
    {
        // Arrange
        var request = new UserDto
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

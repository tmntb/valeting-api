using Service.Models.User;
using Service.Validators.User;

namespace Service.Tests.Validators.User;

public class UpdateEmailValidatorTests
{
    private readonly UpdateEmailValidator _validator;
    
    public UpdateEmailValidatorTests()
    {
        _validator = new UpdateEmailValidator();
    }

    [Fact]
    public void UserId_ShouldFail()
    {        
        // Arrange
        var request = new UserDto
        {
            Id = Guid.Empty
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Id", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    public void Email_ShouldFail(string email)
    {
        // Arrange
        var request = new UserDto
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Email = email
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Email", result.Errors.FirstOrDefault().ErrorMessage);
    }
}

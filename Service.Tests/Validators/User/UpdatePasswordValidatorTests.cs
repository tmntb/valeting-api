using Service.Models.User;
using Service.Validators.User;

namespace Service.Tests.Validators.User;

public class UpdatePasswordValidatorTests
{
    private readonly UpdatePasswordValidator _validator;
    
    public UpdatePasswordValidatorTests()
    {
        _validator = new UpdatePasswordValidator();
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Password_ShouldFail(string? password)
    {
        // Arrange
        var request = new UserDto
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Password = password
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Password", result.Errors.FirstOrDefault().ErrorMessage);
    }
}

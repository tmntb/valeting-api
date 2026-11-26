using Service.Models.Token.Payload;
using Service.Validators.Token;

namespace Service.Tests.Validators.Token;

public class GenerateTokenJwtValidatorTests
{
    private readonly GenerateTokenJwtValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void InvalidEmail_ShouldFail(string? email)
    {
        // Act
        var result = _validator.Validate(new GenerateTokenJwtDtoRequest
        {
            Email = email!
        });
    
        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void ValidEmail_ShouldPass()
    {
        // Act
        var result = _validator.Validate(new GenerateTokenJwtDtoRequest
        {
            Email = "user@example.com"
        });
    
        // Assert
        Assert.True(result.IsValid);
    }
}

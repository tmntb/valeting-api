using Service.Models.Auth.Payload;
using Service.Validators.Auth;

namespace Service.Tests.Validators.Auth;

public class ResetPasswordValidatorTests
{
    private readonly ResetPasswordValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Token_ShouldFail(string? token)
    {
        var result = _validator.Validate(new ResetPasswordDtoRequest
        {
            Token = token!,
            NewPassword = "newpassword123"
        });

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NewPassword_ShouldFail(string? password)
    {
        var result = _validator.Validate(new ResetPasswordDtoRequest
        {
            Token = "reset-token",
            NewPassword = password!
        });

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ValidRequest_ShouldPass()
    {
        var result = _validator.Validate(new ResetPasswordDtoRequest
        {
            Token = "reset-token",
            NewPassword = "newpassword123"
        });

        Assert.True(result.IsValid);
    }
}

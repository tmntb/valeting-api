using Microsoft.AspNetCore.Http;
using Service.Models.Link.Payload;
using Service.Validators;

namespace Service.Tests.Validators;

public class GenerateSelfLinkValidatorTests
{
    private readonly GenerateSelfLinkValidator _validator;

    public GenerateSelfLinkValidatorTests()
    {
        _validator = new GenerateSelfLinkValidator();
    }

    [Fact]
    public void Request_Null_ShouldFail()
    {
        // Arrange
        var request = new GenerateSelfLinkDtoRequest
        {
            Request = null,
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Request", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void GenerateSelfLinkDtoRequest_Valid()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var request = new GenerateSelfLinkDtoRequest
        {
            Request = context.Request
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }
}

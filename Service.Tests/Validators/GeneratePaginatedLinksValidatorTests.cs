using Microsoft.AspNetCore.Http;
using Service.Models.Link.Payload;
using Service.Validators;

namespace Service.Tests.Validators;

public class GeneratePaginatedLinksValidatorTests
{
    private readonly GeneratePaginatedLinksValidator _validator;

    public GeneratePaginatedLinksValidatorTests()
    {
        _validator = new GeneratePaginatedLinksValidator();
    }

    [Fact]
    public void Request_Null_ShouldFail()
    {
        // Arrange
        var request = new GeneratePaginatedLinksDtoRequest
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
    public void GeneratePaginatedLinksDtoRequest_Valid()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var request = new GeneratePaginatedLinksDtoRequest
        {
            Request = context.Request
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }
}

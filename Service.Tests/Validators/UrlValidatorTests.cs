using Microsoft.AspNetCore.Http;
using Valeting.Common.Models.Link;
using Valeting.Core.Validators;

namespace Service.Tests.Validators;

#region GenerateSelfUrlValidatorTests
public class GenerateSelfUrlValidatorTests
{
    private readonly GenerateSelfUrlValidator _validator;

    public GenerateSelfUrlValidatorTests()
    {
        _validator = new GenerateSelfUrlValidator();
    }

    [Fact]
    public void Request_Null_ShouldFail()
    {
        // Arrange
        var request = new GenerateSelfUrlDtoRequest
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
    public void GenerateSelfUrlDtoRequest_Valid()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var request = new GenerateSelfUrlDtoRequest
        {
            Request = context.Request
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }
}
#endregion

#region GeneratePaginatedLinksValidatorTests
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
#endregion

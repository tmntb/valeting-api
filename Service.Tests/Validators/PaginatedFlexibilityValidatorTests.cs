using Service.Models.Flexibility.Payload;
using Service.Validators;

namespace Service.Tests.Validators;

public class PaginatedFlexibilityValidatorTests
{
    private readonly PaginatedFlexibilityValidator _validator;

    public PaginatedFlexibilityValidatorTests()
    {
        _validator = new PaginatedFlexibilityValidator();
    }

    [Fact]
    public void PageNumber_EqualToZero_ShouldFail()
    {
        // Arrange
        var request = new FlexibilityFilterDto
        {
            PageNumber = 0
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Page Number", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void PageNumber_LessThanZero_ShouldFail()
    {
        // Arrange
        var request = new FlexibilityFilterDto
        {
            PageNumber = -1
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Page Number", result.Errors.FirstOrDefault().ErrorMessage);
    }

    [Fact]
    public void PaginatedFlexibilityDtoRequest_Valid()
    {
        // Arrange
        var request = new FlexibilityFilterDto
        {
            PageNumber = 1,
            PageSize = 1
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }
}

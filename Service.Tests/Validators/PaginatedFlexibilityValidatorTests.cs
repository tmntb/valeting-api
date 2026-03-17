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

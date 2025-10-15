using Service.Models.VehicleSize.Payload;
using Service.Validators;

namespace Service.Tests.Validators;

public class PaginatedVehicleSizeValidatorTests
{
    private readonly PaginatedVehicleSizeValidator _validator;

    public PaginatedVehicleSizeValidatorTests()
    {
        _validator = new PaginatedVehicleSizeValidator();
    }

    [Fact]
    public void PageNumber_EqualToZero_ShouldFail()
    {
        // Arrange
        var request = new VehicleSizeFilterDto
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
        var request = new VehicleSizeFilterDto
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
    public void PaginatedVehicleSizeDtoRequest_Valid()
    {
        // Arrange
        var request = new VehicleSizeFilterDto
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

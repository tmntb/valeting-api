using Common.Models.VehicleSize;
using Service.Validators;

namespace Service.Tests.Validators;

#region PaginatedVehicleSizeValidatorTests
public class PaginatedVehicleSizeValidatorTests
{
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly PaginatedVehicleSizeValidator _validator;

    public PaginatedVehicleSizeValidatorTests()
    {
        _validator = new PaginatedVehicleSizeValidator();
    }

    [Fact]
    public void Filter_Empty_ShouldFail()
    {
        // Arrange
        var request = (VehicleSizeFilterDto)null;

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Filter", result.Errors.FirstOrDefault().ErrorMessage);
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
#endregion

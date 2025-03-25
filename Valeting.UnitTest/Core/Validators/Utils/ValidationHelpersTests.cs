using Moq;
using Valeting.Common.Models.Flexibility;
using Valeting.Common.Models.VehicleSize;
using Valeting.Core.Validators.Utils;
using Valeting.Repository.Interfaces;

namespace Valeting.Tests.Core.Validators.Utils;

public class ValidationHelpersTests
{
    private readonly Mock<IFlexibilityRepository> _mockFlexibilityRepository;
    private readonly Mock<IVehicleSizeRepository> _mockVehicleSizeRepository;

    private readonly ValidationHelpers _validationHelpers;

    public ValidationHelpersTests()
    {
        _mockFlexibilityRepository = new Mock<IFlexibilityRepository>();
        _mockVehicleSizeRepository = new Mock<IVehicleSizeRepository>();

        _validationHelpers = new ValidationHelpers(_mockFlexibilityRepository.Object, _mockVehicleSizeRepository.Object);
    }

    [Fact]
    public async Task FlexibilityIsValid_ShouldReturnTrue_WhenFound()
    {
        // Arrange
        var flexibilityId = Guid.NewGuid();
        _mockFlexibilityRepository.Setup(repo => repo.GetByIdAsync(flexibilityId)).ReturnsAsync(new FlexibilityDto());

        // Act
        var result = await _validationHelpers.FlexibilityIsValid(flexibilityId, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task FlexibilityIsValid_ShouldReturnFalse_WhenNotFound()
    {
        // Arrange
        var flexibilityId = Guid.NewGuid();
        _mockFlexibilityRepository.Setup(repo => repo.GetByIdAsync(flexibilityId)).ReturnsAsync((FlexibilityDto)null);

        // Act
        var result = await _validationHelpers.FlexibilityIsValid(flexibilityId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task VehicleSizeIsValid_ShouldReturnTrue_WhenFound()
    {
        // Arrange
        var vehicleSizeId = Guid.NewGuid();
        _mockVehicleSizeRepository.Setup(repo => repo.GetByIdAsync(vehicleSizeId)).ReturnsAsync(new VehicleSizeDto());

        // Act
        var result = await _validationHelpers.VehicleSizeIsValid(vehicleSizeId, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task VehicleSizeIsValid_ShouldReturnFalse_WhenNotFound()
    {
        // Arrange
        var vehicleSizeId = Guid.NewGuid();
        _mockVehicleSizeRepository.Setup(repo => repo.GetByIdAsync(vehicleSizeId)).ReturnsAsync((VehicleSizeDto)null);

        // Act
        var result = await _validationHelpers.VehicleSizeIsValid(vehicleSizeId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}

using Moq;
using Common.Models.Flexibility;
using Common.Models.VehicleSize;
using Service.Interfaces;
using Service.Validators.Utils;

namespace Service.Tests.Validators.Utils;

public class ValidationHelpersTests
{
    private readonly Mock<IFlexibilityRepository> _mockFlexibilityRepository;
    private readonly Mock<IVehicleSizeRepository> _mockVehicleSizeRepository;

    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");
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
        _mockFlexibilityRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new FlexibilityDto());

        // Act
        var result = await _validationHelpers.FlexibilityIsValid(_mockId, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task FlexibilityIsValid_ShouldReturnFalse_WhenNotFound()
    {
        // Arrange
        _mockFlexibilityRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((FlexibilityDto)null);

        // Act
        var result = await _validationHelpers.FlexibilityIsValid(_mockId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task VehicleSizeIsValid_ShouldReturnTrue_WhenFound()
    {
        // Arrange
        _mockVehicleSizeRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new VehicleSizeDto());

        // Act
        var result = await _validationHelpers.VehicleSizeIsValid(_mockId, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task VehicleSizeIsValid_ShouldReturnFalse_WhenNotFound()
    {
        // Arrange
        _mockVehicleSizeRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((VehicleSizeDto)null);

        // Act
        var result = await _validationHelpers.VehicleSizeIsValid(_mockId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}

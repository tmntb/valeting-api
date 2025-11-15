using Integration.Tests.Repository;
using Repository.Entities;
using Repository.Repositories;

namespace Integration.Tests.Repository;

public class VehicleSizeRepositoryTests : BaseRepositoryTest
{
    private readonly VehicleSizeRepository _vehicleSizeRepository;
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000031");

    public VehicleSizeRepositoryTests()
    {
        _vehicleSizeRepository = new VehicleSizeRepository(Context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenRdVehicleSizeDoesNotExists()
    {
        // Act
        var result = await _vehicleSizeRepository.GetByIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000039"));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnVehicleSizeDtoWhenRdVehicleSizeExists()
    {
        // Act
        var result = await _vehicleSizeRepository.GetByIdAsync(_mockId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_mockId, result.Id);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnListAllRecords_WhenActiveQueryFilterIsNull()
    {
        // Arrange
        var vehicleSize = new RdVehicleSize
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000032"),
            Description = "Van",
            Active = false,
        };

        Context.RdVehicleSizes.Add(vehicleSize);
        await Context.SaveChangesAsync();

        // Act
        var result = await _vehicleSizeRepository.GetFilteredAsync(new());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnFilteredList_WhenActiveQueryFilterIsTrue()
    {
        // Arrange
        var vehicleSize = new RdVehicleSize
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000033"),
            Description = "Van",
            Active = false,
        };

        Context.RdVehicleSizes.Add(vehicleSize);
        await Context.SaveChangesAsync();

        // Act
        var result = await _vehicleSizeRepository.GetFilteredAsync(
            new()
            {
                Active = true
            });

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnFilteredList_WhenActiveQueryFilterIsFalse()
    {
        // Arrange
        var vehicleSize = new RdVehicleSize
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000034"),
            Description = "Van",
            Active = false,
        };

        Context.RdVehicleSizes.Add(vehicleSize);
        await Context.SaveChangesAsync();

        // Act
        var result = await _vehicleSizeRepository.GetFilteredAsync(
            new()
            {
                Active = false
            });

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }
}

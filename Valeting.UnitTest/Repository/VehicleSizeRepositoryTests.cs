using Microsoft.EntityFrameworkCore;
using Moq;
using Valeting.Common.Models.VehicleSize;
using Valeting.Repository.Entities;
using Valeting.Repository.Repositories;

namespace Valeting.Tests.Repository;

public class VehicleSizeRepositoryTests
{
    private readonly ValetingContext _valetingContext;
    private readonly VehicleSizeRepository _vehicleSizeRepository;
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    
    public VehicleSizeRepositoryTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<ValetingContext>()
          .UseInMemoryDatabase(databaseName: "ValetingTestDb")
          .Options;

        _valetingContext = new ValetingContext(dbContextOptions);
        _vehicleSizeRepository = new VehicleSizeRepository(_valetingContext);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenRdVehicleSizeDoesNotExists()
    {
        // Act
        var result = await _vehicleSizeRepository.GetByIdAsync(_mockId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnVehicleSizeDtoWhenRdVehicleSizeExists()
    {
        // Arrange
        _valetingContext.RdVehicleSizes.Add(
            new RdVehicleSize
            {
                Id = _mockId,
                Description = "description",
                Active = true,
            });
        await _valetingContext.SaveChangesAsync();

        // Act
        var result = await _vehicleSizeRepository.GetByIdAsync(_mockId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_mockId, result.Id);

        // Clear data
        var clearData = _valetingContext.RdVehicleSizes;
        _valetingContext.RdVehicleSizes.RemoveRange(clearData);
        await _valetingContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnListAllRecords_WhenActiveQueryFilterIsNull()
    {
        // Arrange
        _valetingContext.RdVehicleSizes.AddRange(
            new List<RdVehicleSize>
            {
                new()
                {
                    Id = _mockId,
                    Description = "description",
                    Active = true,
                },
                new()
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Description = "description",
                    Active = false,
                }
            });
        await _valetingContext.SaveChangesAsync();

        // Act
        var result = await _vehicleSizeRepository.GetFilteredAsync(new());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        // Clear data
        var clearData = _valetingContext.RdVehicleSizes;
        _valetingContext.RdVehicleSizes.RemoveRange(clearData);
        await _valetingContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnFilteredList_WhenActiveQueryFilterIsTrue()
    {
        // Arrange
        _valetingContext.RdVehicleSizes.AddRange(
            new List<RdVehicleSize>
            {
                new()
                {
                    Id = _mockId,
                    Description = "description",
                    Active = true,
                },
                new()
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Description = "description",
                    Active = false,
                }
            });
        await _valetingContext.SaveChangesAsync();

        // Act
        var result = await _vehicleSizeRepository.GetFilteredAsync(
            new()
            {
                Active = true
            });

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);

        // Clear data
        var clearData = _valetingContext.RdVehicleSizes;
        _valetingContext.RdVehicleSizes.RemoveRange(clearData);
        await _valetingContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnFilteredList_WhenActiveQueryFilterIsFalse()
    {
        // Arrange
        _valetingContext.RdVehicleSizes.AddRange(
            new List<RdVehicleSize>
            {
                new()
                {
                    Id = _mockId,
                    Description = "description",
                    Active = true,
                },
                new()
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Description = "description",
                    Active = false,
                }
            });
        await _valetingContext.SaveChangesAsync();

        // Act
        var result = await _vehicleSizeRepository.GetFilteredAsync(
            new()
            {
                Active = false
            });

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);

        // Clear data
        var clearData = _valetingContext.RdVehicleSizes;
        _valetingContext.RdVehicleSizes.RemoveRange(clearData);
        await _valetingContext.SaveChangesAsync();
    }
}

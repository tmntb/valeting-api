using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Valeting.Common.Models.Flexibility;
using Valeting.Repository.Entities;
using Valeting.Repository.Repositories;

namespace Valeting.Tests.Repository;

public class FlexibilityRepositoryTests
{
    private readonly Mock<IMapper> _mockMapper;

    private readonly ValetingContext _valetingContext;
    private readonly FlexibilityRepository _flexibilityRepository;
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public FlexibilityRepositoryTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<ValetingContext>()
           .UseInMemoryDatabase(databaseName: "ValetingTestDb")
           .Options;

        _mockMapper = new Mock<IMapper>();

        _valetingContext = new ValetingContext(dbContextOptions);
        _flexibilityRepository = new FlexibilityRepository(_valetingContext, _mockMapper.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenRdFlexibilityDoesNotExists()
    {
        // Act
        var result = await _flexibilityRepository.GetByIdAsync(_mockId);

        // Assert
        Assert.Null(result);
        _mockMapper.Verify(m => m.Map<FlexibilityDto>(It.IsAny<RdFlexibility>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnFlexibilityDtoWhenRdFlexibilityExists()
    {
        // Arrange
        _valetingContext.RdFlexibilities.Add(new RdFlexibility
        {
            Id = _mockId,
            Description = "description",
            Active = true,
        });
        await _valetingContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<FlexibilityDto>(It.IsAny<RdFlexibility>()))
            .Returns(
                new FlexibilityDto
                {
                    Id = _mockId
                });

        // Act
        var result = await _flexibilityRepository.GetByIdAsync(_mockId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_mockId, result.Id);
        _mockMapper.Verify(m => m.Map<FlexibilityDto>(It.IsAny<RdFlexibility>()), Times.Once);

        // Clear data
        var clearData = _valetingContext.RdFlexibilities;
        _valetingContext.RdFlexibilities.RemoveRange(clearData);
        await _valetingContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnListAllRecords_WhenActiveQueryFilterIsNull()
    {
        // Arrange
        _valetingContext.RdFlexibilities.AddRange(
            new List<RdFlexibility>
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

        _mockMapper.Setup(m => m.Map<List<FlexibilityDto>>(It.IsAny<IEnumerable<RdFlexibility>>()))
            .Returns(
                new List<FlexibilityDto>()
                {
                    new()
                    {
                        Id = _mockId
                    },
                    new()
                    {
                        Id = Guid.Parse("00000000-0000-0000-0000-000000000002")
                    }
                });

        // Act
        var result = await _flexibilityRepository.GetFilteredAsync(new());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        _mockMapper.Verify(m => m.Map<List<FlexibilityDto>>(It.IsAny<IEnumerable<RdFlexibility>>()), Times.Once);

        // Clear data
        var clearData = _valetingContext.RdFlexibilities;
        _valetingContext.RdFlexibilities.RemoveRange(clearData);
        await _valetingContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnFilteredList_WhenActiveQueryFilterIsTrue()
    {
        // Arrange
        _valetingContext.RdFlexibilities.AddRange(
            new List<RdFlexibility>
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

        _mockMapper.Setup(m => m.Map<List<FlexibilityDto>>(It.IsAny<IEnumerable<RdFlexibility>>()))
            .Returns(
                new List<FlexibilityDto>()
                {
                    new()
                    {
                        Id = _mockId
                    }
                });

        // Act
        var result = await _flexibilityRepository.GetFilteredAsync(
            new()
            {
                Active = true
            });

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        _mockMapper.Verify(m => m.Map<List<FlexibilityDto>>(It.IsAny<IEnumerable<RdFlexibility>>()), Times.Once);

        // Clear data
        var clearData = _valetingContext.RdFlexibilities;
        _valetingContext.RdFlexibilities.RemoveRange(clearData);
        await _valetingContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnFilteredList_WhenActiveQueryFilterIsFalse()
    {
        // Arrange
        _valetingContext.RdFlexibilities.AddRange(
            new List<RdFlexibility>
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

        _mockMapper.Setup(m => m.Map<List<FlexibilityDto>>(It.IsAny<IEnumerable<RdFlexibility>>()))
            .Returns(
                new List<FlexibilityDto>()
                {
                    new()
                    {
                        Id = Guid.Parse("00000000-0000-0000-0000-000000000002")
                    }
                });

        // Act
        var result = await _flexibilityRepository.GetFilteredAsync(
            new()
            {
                Active = false
            });

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        _mockMapper.Verify(m => m.Map<List<FlexibilityDto>>(It.IsAny<IEnumerable<RdFlexibility>>()), Times.Once);

        // Clear data
        var clearData = _valetingContext.RdFlexibilities;
        _valetingContext.RdFlexibilities.RemoveRange(clearData);
        await _valetingContext.SaveChangesAsync();
    }
}

using Valeting.Repository.Entities;
using Valeting.Repository.Repositories;

namespace Integration.Tests.Repository;

public class FlexibilityRepositoryTests : BaseRepositoryTest
{
    private readonly FlexibilityRepository _flexibilityRepository;
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000021");

    public FlexibilityRepositoryTests()
    {
        _flexibilityRepository = new FlexibilityRepository(Context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenRdFlexibilityDoesNotExists()
    {
        // Act
        var result = await _flexibilityRepository.GetByIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000029"));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnFlexibilityDtoWhenRdFlexibilityExists()
    {
        // Act
        var result = await _flexibilityRepository.GetByIdAsync(_mockId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_mockId, result.Id);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnListAllRecords_WhenActiveQueryFilterIsNull()
    {
        // Arrange
        var flexibility = new RdFlexibility
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000022"),
            Description = "2 Days",
            Active = false
        };

        Context.RdFlexibilities.Add(flexibility);
        await Context.SaveChangesAsync();

        // Act
        var result = await _flexibilityRepository.GetFilteredAsync(new());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnFilteredList_WhenActiveQueryFilterIsTrue()
    {
        // Arrange
        var flexibility = new RdFlexibility
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000022"),
            Description = "2 Days",
            Active = false
        };

        Context.RdFlexibilities.Add(flexibility);
        await Context.SaveChangesAsync();

        // Act
        var result = await _flexibilityRepository.GetFilteredAsync(
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
        var flexibility = new RdFlexibility
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000022"),
            Description = "2 Days",
            Active = false
        };

        Context.RdFlexibilities.Add(flexibility);
        await Context.SaveChangesAsync();

        // Act
        var result = await _flexibilityRepository.GetFilteredAsync(
            new()
            {
                Active = false
            });

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }
}

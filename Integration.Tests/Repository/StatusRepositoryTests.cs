using Repository.Repositories;

namespace Integration.Tests.Repository;

public class StatusRepositoryTests : BaseRepositoryTest
{
    private readonly StatusRepository _statusRepository;

    public StatusRepositoryTests()
    {
        _statusRepository = new StatusRepository(Context);
    }

    [Fact]
    public async Task GetByCodeAsync_ShouldReturnNull_WhenStatusDoesNotExists()
    {
        // Act
        var result = await _statusRepository.GetByCodeAsync(Common.Enums.StatusEnum.COMPLETED);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByCodeAsync_ShouldReturnStatusDtoWhenStatusExists()
    {
        // Act
        var result = await _statusRepository.GetByCodeAsync(Common.Enums.StatusEnum.PENDING_APPROVAL);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(DataFactory.STATUS_ID, result.Id);
    }
}

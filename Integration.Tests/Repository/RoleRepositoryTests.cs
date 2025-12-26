using Common.Enums;
using Repository.Repositories;

namespace Integration.Tests.Repository;

public class RoleRepositoryTests : BaseRepositoryTest
{
    private readonly RoleRepository _roleRepository;
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000051");

    public RoleRepositoryTests()
    {
        _roleRepository = new RoleRepository(Context);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnNull_WhenRoleDoesNotExists()
    {
        // Act
        var result = await _roleRepository.GetByNameAsync(RoleEnum.Admin);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnRoleDtoWhenRoleExists()
    {
        // Act
        var result = await _roleRepository.GetByNameAsync(RoleEnum.User);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_mockId, result.Id);
    }
}

using Common.Enums;
using Repository.Repositories;

namespace Integration.Tests.Repository;

public class RoleRepositoryTests : BaseRepositoryTest
{
    private readonly RoleRepository _roleRepository;

    public RoleRepositoryTests()
    {
        _roleRepository = new RoleRepository(Context);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnNull_WhenRoleDoesNotExists()
    {
        // Act
        var result = await _roleRepository.GetByCodeAsync(RoleEnum.ADMIN);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnRoleDtoWhenRoleExists()
    {
        // Act
        var result = await _roleRepository.GetByCodeAsync(RoleEnum.USER);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(DataFactory.ROLE_ID, result.Id);
    }
}

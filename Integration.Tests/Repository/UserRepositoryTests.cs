using Repository.Repositories;

namespace Integration.Tests.Repository;

public class UserRepositoryTests : BaseRepositoryTest
{

    private readonly UserRepository _userRepository;
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000041");

    public UserRepositoryTests()
    {
        _userRepository = new UserRepository(Context);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnNull_WhenApplicationUsersDoesNotExists()
    {
        // Act
        var result = await _userRepository.GetUserByEmailAsync("username49");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnUserDtoWhenApplicationUserExists()
    {
        // Act
        var result = await _userRepository.GetUserByEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_mockId, result.Id);
    }

    [Fact]
    public async Task RegisterAsync_ShouldAddUserToDatabase()
    {
        // Act
        await _userRepository.RegisterAsync(
            new()
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000042"),
                Username = "username42",
                PasswordHash = "password42",
                Email = "test@example.com"
            });

        var result = await Context.ApplicationUsers.FindAsync("username");

        // Assert
        Assert.NotNull(result);
    }
}

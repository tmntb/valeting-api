using Common.Enums;
using Repository.Repositories;
using Service.Models.User;

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
        var result = await _userRepository.GetUserByEmailAsync("test1@example.com");

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
        // Arrange
        var id = Guid.Parse("00000000-0000-0000-0000-000000000042");
        // Act
        await _userRepository.RegisterAsync(
            new()
            {
                Id = id,
                Username = "username42",
                PasswordHash = "password42",
                Email = "test1@example.com",
                ContactNumber = 123456789,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.MinValue,
                LastLoginAt = DateTime.MinValue,
                Role = new()
                { Id = Guid.Parse("00000000-0000-0000-0000-000000000051"), Name = RoleEnum.User },
                IsActive = true
            });

        var result = await Context.ApplicationUsers.FindAsync(id);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenNoUserForGivenIdExists()
    {
        // Arrange
        var userDto = new UserDto
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000043"),
        };

        // Act & Assert
        await _userRepository.UpdateAsync(userDto);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingUserInDatabase()
    {
        // Arrange
        var userDto = new UserDto
        {
            Id = _mockId,
            Username = "updatedUsername",
            PasswordHash = "updatedPassword",
            ContactNumber = 987654321,
            Email = "test@example.com",
            Role = new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000051"), Name = RoleEnum.User },
            IsActive = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.MinValue
        };

        // Act
        await _userRepository.UpdateAsync(userDto);

        // Assert
        var updatedUser = await Context.ApplicationUsers.FindAsync(_mockId);
        Assert.NotNull(updatedUser);
        Assert.Equal("updatedUsername", updatedUser.Username);
        Assert.Equal("updatedPassword", updatedUser.PasswordHash);
        Assert.Equal(987654321, updatedUser.ContactNumber);
        Assert.False(updatedUser.IsActive);
        Assert.Equal(userDto.UpdatedAt, updatedUser.UpdatedAt);
    }
}

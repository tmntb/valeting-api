using Common.Enums;
using Repository.Repositories;
using Service.Models.User;

namespace Integration.Tests.Repository;

public class UserRepositoryTests : BaseRepositoryTest
{
    private readonly UserRepository _userRepository;

    public UserRepositoryTests()
    {
        _userRepository = new UserRepository(Context);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnNull_WhenApplicationUsersDoesNotExists()
    {
        // Act
        var result = await _userRepository.GetByEmailAsync("test1@example.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnUserDtoWhenApplicationUserExists()
    {
        // Act
        var result = await _userRepository.GetByEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(DataFactory.USER_ID, result.Id);
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
                Email = "test1@example.com",
                PasswordHash = "password42",
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateOnly(1957, 3, 25),
                ContactNumber = 123456789,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.MinValue,
                LastLoginAt = DateTime.MinValue,
                Role = new()
                { Id = Guid.Parse("00000000-0000-0000-0000-000000000051"), Code = RoleEnum.USER },
                IsActive = true
            });

        var result = await Context.ApplicationUsers.FindAsync(id);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAdminSettingsAsync_ShouldReturnNull_WhenNoUserForGivenIdExists()
    {
        // Arrange
        var userDto = new UserDto
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000043"),
        };

        // Act & Assert
        await _userRepository.UpdateAdminSettingsAsync(userDto);
    }

    [Fact]
    public async Task UpdateAdminSettingsAsync_ShouldUpdateExistingUserInDatabase()
    {
        // Arrange
        var userDto = new UserDto
        {
            Id = DataFactory.USER_ID,
            Role = new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000051"), Code = RoleEnum.USER },
            IsActive = false,
        };

        // Act
        await _userRepository.UpdateAdminSettingsAsync(userDto);

        // Assert
        var updatedUser = await Context.ApplicationUsers.FindAsync(DataFactory.USER_ID);
        Assert.NotNull(updatedUser);
        Assert.False(updatedUser.IsActive);
        Assert.Equal(Guid.Parse("00000000-0000-0000-0000-000000000051"), updatedUser.RoleId);
    }
}

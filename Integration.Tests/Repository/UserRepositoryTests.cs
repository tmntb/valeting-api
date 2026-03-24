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
            Id = DataFactory.USER_ID,
            Email = "test@example.com",
            PasswordHash = "updatedPassword",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateOnly(1957, 3, 25),
            ContactNumber = 987654321,
            Role = new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000051"), Code = RoleEnum.USER },
            IsActive = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.MinValue
        };

        // Act
        await _userRepository.UpdateAsync(userDto);

        // Assert
        var updatedUser = await Context.ApplicationUsers.FindAsync(DataFactory.USER_ID);
        Assert.NotNull(updatedUser);
        Assert.Equal("updatedPassword", updatedUser.PasswordHash);
        Assert.Equal(987654321, updatedUser.ContactNumber);
        Assert.False(updatedUser.IsActive);
        Assert.Equal(userDto.UpdatedAt, updatedUser.UpdatedAt);
    }
}

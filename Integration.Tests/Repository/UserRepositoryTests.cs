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
    public async Task GetUserByIdAsync_ShouldReturnNull_WhenApplicationUsersDoesNotExists()
    {
        // Act
        var result = await _userRepository.GetByIdAsync(Guid.Parse("00000000-0000-0000-0000-000000000042"));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUserDtoWhenApplicationUserExists()
    {
        // Act
        var result = await _userRepository.GetByIdAsync(DataFactory.USER_ID);

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

    [Fact]
    public async Task UpdateEmailAsync_ShouldReturnNull_WhenNoUserForGivenIdExists()
    {
        // Arrange
        var userDto = new UserDto
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000043")
        };

        // Act & Assert
        await _userRepository.UpdateEmailAsync(userDto);
    }

    [Fact]
    public async Task UpdateEmailAsync_ShouldUpdateExistingUserInDatabase()
    {
        // Arrange
        var userDto = new UserDto
        {
            Id = DataFactory.USER_ID,
            Email = "updated@example.com"
        };

        // Act
        await _userRepository.UpdateEmailAsync(userDto);

        // Assert
        var updatedUser = await Context.ApplicationUsers.FindAsync(DataFactory.USER_ID);
        Assert.NotNull(updatedUser);
        Assert.Equal("updated@example.com", updatedUser.Email);
    }

    [Fact]
    public async Task UpdateLastLoginAsync_ShouldReturnNull_WhenNoUserForGivenIdExists()
    {
        // Arrange
        var id = Guid.Parse("00000000-0000-0000-0000-000000000043");

        // Act & Assert
        await _userRepository.UpdateLastLoginAsync(id);
    }

    [Fact]
    public async Task UpdateLastLoginAsync_ShouldUpdateExistingUserInDatabase()
    {
        // Act
        await _userRepository.UpdateLastLoginAsync(DataFactory.USER_ID);

        // Assert
        var updatedUser = await Context.ApplicationUsers.FindAsync(DataFactory.USER_ID);
        Assert.NotNull(updatedUser);
        Assert.True(updatedUser.LastLoginAt > DateTime.MinValue);
    }

    [Fact]
    public async Task UpdateMfaEnableAsync_ShouldReturnNull_WhenNoUserForGivenIdExists()
    {
        // Arrange
        var userDto = new UserDto
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000043")
        };

        // Act & Assert
        await _userRepository.UpdateMfaEnableAsync(userDto);
    }

    [Fact]
    public async Task UpdateMfaEnableAsync_ShouldUpdateExistingUserInDatabase()
    {
        // Arrange
        var userDto = new UserDto
        {
            Id = DataFactory.USER_ID,
            MfaEnabled = true
        };

        // Act
        await _userRepository.UpdateMfaEnableAsync(userDto);

        // Assert
        var updatedUser = await Context.ApplicationUsers.FindAsync(DataFactory.USER_ID);
        Assert.NotNull(updatedUser);
        Assert.True(updatedUser.MfaEnabled);
    }

    [Fact]
    public async Task UpdateMfaSecretAsync_ShouldReturnNull_WhenNoUserForGivenIdExists()
    {
        // Arrange
        var userDto = new UserDto
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000043")
        };

        // Act & Assert
        await _userRepository.UpdateMfaSecretAsync(userDto);
    }

    [Fact]
    public async Task UpdateMfaSecretAsync_ShouldUpdateExistingUserInDatabase()
    {
        // Arrange
        var userDto = new UserDto
        {
            Id = DataFactory.USER_ID,
            MfaSecret = "mfaSecret"
        };

        // Act
        await _userRepository.UpdateMfaSecretAsync(userDto);

        // Assert
        var updatedUser = await Context.ApplicationUsers.FindAsync(DataFactory.USER_ID);
        Assert.NotNull(updatedUser);
        Assert.Equal("mfaSecret", updatedUser.MfaSecret);
    }

    [Fact]
    public async Task UpdatePasswordAsync_ShouldReturnNull_WhenNoUserForGivenIdExists()
    {
        // Arrange
        var userDto = new UserDto
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000043")
        }; 

        // Act & Assert
        await _userRepository.UpdatePasswordAsync(userDto);
    }

    [Fact]
    public async Task UpdatePasswordAsync_ShouldUpdateExistingUserInDatabase()
    {
        // Arrange
        var userDto = new UserDto
        {
            Id = DataFactory.USER_ID,
            PasswordHash = "newpasswordhash"
        };

        // Act
        await _userRepository.UpdatePasswordAsync(userDto);

        // Assert
        var updatedUser = await Context.ApplicationUsers.FindAsync(DataFactory.USER_ID);
        Assert.NotNull(updatedUser);
        Assert.Equal("newpasswordhash", updatedUser.PasswordHash);
    }

    [Fact]
    public async Task UpdateProfileAsync_ShouldReturnNull_WhenNoUserForGivenIdExists()
    {
        // Arrange
        var userDto = new UserDto
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000043")
        };

        // Act & Assert
        await _userRepository.UpdateProfileAsync(userDto);
    }

    [Fact]
    public async Task UpdateProfileAsync_ShouldUpdateExistingUserInDatabase()
    {
        // Arrange
        var userDto = new UserDto
        {
            Id = DataFactory.USER_ID,
            FirstName = "UpdatedFirstName",
            LastName = "UpdatedLastName",
            DateOfBirth = new DateOnly(1990, 1, 1),
            ContactNumber = 987654321
        };

        // Act
        await _userRepository.UpdateProfileAsync(userDto);

        // Assert
        var updatedUser = await Context.ApplicationUsers.FindAsync(DataFactory.USER_ID);
        Assert.NotNull(updatedUser);
        Assert.Equal("UpdatedFirstName", updatedUser.FirstName);
        Assert.Equal("UpdatedLastName", updatedUser.LastName);
        Assert.Equal(new DateOnly(1990, 1, 1), updatedUser.DateOfBirth);
        Assert.Equal(987654321, updatedUser.ContactNumber);
    }
}


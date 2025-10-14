using Microsoft.EntityFrameworkCore;
using Moq;
using Valeting.Common.Models.User;
using Valeting.Repository.Entities;
using Valeting.Repository.Repositories;

namespace Valeting.Tests.Repository;

public class UserRepositoryTests
{

    private readonly ValetingContext _valetingContext;
    private readonly UserRepository _userRepository;
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public UserRepositoryTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<ValetingContext>()
           .UseInMemoryDatabase(databaseName: "ValetingTestDb")
           .Options;

        _valetingContext = new ValetingContext(dbContextOptions);
        _userRepository = new UserRepository(_valetingContext);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnNull_WhenApplicationUsersDoesNotExists()
    {
        // Act
        var result = await _userRepository.GetUserByEmailAsync("username");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnUserDtoWhenApplicationUserExists()
    {
        // Arrange
        _valetingContext.ApplicationUsers.Add(
            new ApplicationUser
            {
                Id = _mockId,
                Username = "username",
                Password = "password"
            });
        await _valetingContext.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetUserByEmailAsync("username");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_mockId, result.Id);

        // Clear data
        var clearData = _valetingContext.ApplicationUsers;
        _valetingContext.ApplicationUsers.RemoveRange(clearData);
        await _valetingContext.SaveChangesAsync();
    }

    [Fact]
    public async Task RegisterAsync_ShouldAddUserToDatabase()
    {
        // Act
        await _userRepository.RegisterAsync(
            new()
            {
                Id = _mockId,
                Username = "username",
                Password = "password"
            });

        var result = await _valetingContext.ApplicationUsers.FindAsync("username");

        // Assert
        Assert.NotNull(result);

        // Clear data
        _valetingContext.ApplicationUsers.Remove(result);
        await _valetingContext.SaveChangesAsync();
    }
}

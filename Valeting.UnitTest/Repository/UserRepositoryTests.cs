using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Valeting.Common.Models.Booking;
using Valeting.Common.Models.Flexibility;
using Valeting.Common.Models.User;
using Valeting.Repository.Entities;
using Valeting.Repository.Interfaces;
using Valeting.Repository.Repositories;

namespace Valeting.Tests.Repository;

public class UserRepositoryTests
{
    private readonly Mock<IMapper> _mockMapper;

    private readonly ValetingContext _valetingContext;
    private readonly UserRepository _userRepository;
    private readonly Guid _mockId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public UserRepositoryTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<ValetingContext>()
           .UseInMemoryDatabase(databaseName: "ValetingTestDb")
           .Options;

        _mockMapper = new Mock<IMapper>();

        _valetingContext = new ValetingContext(dbContextOptions);
        _userRepository = new UserRepository(_valetingContext, _mockMapper.Object);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnNull_WhenApplicationUsersDoesNotExists()
    {
        // Act
        var result = await _userRepository.GetUserByEmailAsync("username");

        // Assert
        Assert.Null(result);
        _mockMapper.Verify(m => m.Map<UserDto>(It.IsAny<ApplicationUser>()), Times.Never);
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

        _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<ApplicationUser>()))
            .Returns(
                new UserDto
                {
                    Id = _mockId
                });

        // Act
        var result = await _userRepository.GetUserByEmailAsync("username");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_mockId, result.Id);
        _mockMapper.Verify(m => m.Map<UserDto>(It.IsAny<ApplicationUser>()), Times.Once);

        // Clear data
        var clearData = _valetingContext.ApplicationUsers;
        _valetingContext.ApplicationUsers.RemoveRange(clearData);
        await _valetingContext.SaveChangesAsync();
    }

    [Fact]
    public async Task RegisterAsync_ShouldAddUserToDatabase()
    {
        // Arrange
        _mockMapper.Setup(m => m.Map<ApplicationUser>(It.IsAny<UserDto>()))
            .Returns(
                new ApplicationUser
                {
                    Id = _mockId,
                    Username = "username",
                    Password = "password"
                });

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
        _mockMapper.Verify(m => m.Map<ApplicationUser>(It.IsAny<UserDto>()), Times.Once);

        // Clear data
        _valetingContext.ApplicationUsers.Remove(result);
        await _valetingContext.SaveChangesAsync();
    }
}

using Common.Enums;
using Common.Messages;
using Moq;
using Service.Interfaces;
using Service.Models.Role;
using Service.Models.User;
using Service.Services;

namespace Service.Tests.Services;

public class AuthServiceTests
{
    private readonly UserDto _userDto;

    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRecoveryCodeRepository> _mockRecoveryCodeRepository;
    private readonly Mock<IRoleRepository> _mockRoleRepository;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userDto = DataFactory.CreateUserDto();
        
        _mockUserRepository = new Mock<IUserRepository>();
        _mockRecoveryCodeRepository = new Mock<IRecoveryCodeRepository>();
        _mockRoleRepository = new Mock<IRoleRepository>();

        _authService = new AuthService(_mockUserRepository.Object, _mockRecoveryCodeRepository.Object, _mockRoleRepository.Object);
    }
    
    [Fact]
    public async Task RegisterAsync_ShouldThrowInvalidOperationException_WhenUserExists()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_userDto);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.RegisterAsync(_userDto));

        Assert.Equal(exception.Message, Messages.EmailInUse);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_WhenValid()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDto)null);

        _mockRoleRepository
            .Setup(repo => repo.GetByCodeAsync(It.IsAny<RoleEnum>()))
            .ReturnsAsync(new RoleDto
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Code = RoleEnum.USER
            });

        // Act
        await _authService.RegisterAsync(new()
        {
            Email = "user@example.com",
            Password = "password",
            FirstName = "John1",
            LastName = "Doe",
            DateOfBirth = new DateOnly(1930, 3, 7),
            ContactNumber = 123456789,
            Role = new()
            {
                Code = RoleEnum.USER
            }
        });

        // Assert
        _mockUserRepository.Verify(repo => repo.RegisterAsync(It.IsAny<UserDto>()), Times.Once);
    }
}

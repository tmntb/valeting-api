using AutoMapper;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Valeting.API.Controllers;
using Valeting.API.Models.User;
using Valeting.Common.Messages;
using Valeting.Common.Models.User;
using Valeting.Core.Interfaces;

namespace Valeting.Tests.API;

public class UserControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IMapper> _mockMapper;

    private readonly UserController _userController;

    public UserControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockMapper = new Mock<IMapper>();
        _userController = new UserController(_mockUserService.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        var loginRequest = new LoginApiRequest { Username = "test@example.com", Password = "password" };

        var validateRequest = new ValidateLoginDtoRequest();
        _mockMapper.Setup(m => m.Map<ValidateLoginDtoRequest>(It.IsAny<LoginApiRequest>())).Returns(validateRequest);

        var validateResponse = new ValidateLoginDtoResponse { Valid = true };
        _mockUserService.Setup(s => s.ValidateLoginAsync(It.IsAny<ValidateLoginDtoRequest>())).ReturnsAsync(validateResponse);

        var generateRequest = new GenerateTokenJWTDtoRequest();
        _mockMapper.Setup(m => m.Map<GenerateTokenJWTDtoRequest>(It.IsAny<LoginApiRequest>())).Returns(generateRequest);

        var generateResponse = new GenerateTokenJWTDtoResponse { Token = "valid_token" };
        _mockUserService.Setup(s => s.GenerateTokenJWTAsync(It.IsAny<GenerateTokenJWTDtoRequest>())).ReturnsAsync(generateResponse);

        var loginResponse = new LoginApiResponse { Token = "valid_token", ExpiryDate = It.IsAny<DateTime>(), TokenType = It.IsAny<string>() };
        _mockMapper.Setup(m => m.Map<LoginApiResponse>(It.IsAny<GenerateTokenJWTDtoResponse>())).Returns(loginResponse);

        // Act
        var result = await _userController.Login(loginRequest) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(loginResponse, result.Value);
    }

    [Fact]
    public async Task Login_ShouldThrowUnauthorizedAccessException_WhenInvalidCredentials()
    {
        // Arrange
        var loginRequest = new LoginApiRequest { Username = "test@example.com", Password = "wrongpassword" };

        var validateRequest = new ValidateLoginDtoRequest();
        _mockMapper.Setup(m => m.Map<ValidateLoginDtoRequest>(It.IsAny<LoginApiRequest>())).Returns(validateRequest);

        var validateResponse = new ValidateLoginDtoResponse { Valid = false };
        _mockUserService.Setup(s => s.ValidateLoginAsync(It.IsAny<ValidateLoginDtoRequest>())).ReturnsAsync(validateResponse);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userController.Login(loginRequest));
        Assert.Equal(Messages.InvalidPassword, exception.Message);
    }

    [Fact]
    public async Task Login_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _userController.Login(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var registerRequest = new RegisterApiRequest { Username = "test@example.com", Password = "password" };
        
        var registerDtoRequest = new RegisterDtoRequest();
        _mockMapper.Setup(m => m.Map<RegisterDtoRequest>(It.IsAny<RegisterApiRequest>())).Returns(registerDtoRequest);

        _mockUserService.Setup(s => s.RegisterAsync(It.IsAny<RegisterDtoRequest>())).Returns(Task.CompletedTask);

        // Act
        var result = await _userController.Register(registerRequest) as StatusCodeResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task Register_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _userController.Register(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }
}

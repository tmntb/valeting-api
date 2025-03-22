using AutoMapper;
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
    public async Task Login_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _userController.Login(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task Login_ShouldThrowUnauthorizedAccessException_WhenInvalidCredentials()
    {
        // Arrange
        var loginRequest = new LoginApiRequest { Username = "test@example.com", Password = "wrongpassword" };

        _mockMapper.Setup(m => m.Map<ValidateLoginDtoRequest>(It.IsAny<LoginApiRequest>())).Returns(new ValidateLoginDtoRequest());
        _mockUserService.Setup(s => s.ValidateLoginAsync(It.IsAny<ValidateLoginDtoRequest>())).ReturnsAsync(new ValidateLoginDtoResponse { Valid = false });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userController.Login(loginRequest));
        Assert.Equal(Messages.InvalidPassword, exception.Message);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        _mockMapper.Setup(m => m.Map<ValidateLoginDtoRequest>(It.IsAny<LoginApiRequest>())).Returns(new ValidateLoginDtoRequest());
        _mockUserService.Setup(s => s.ValidateLoginAsync(It.IsAny<ValidateLoginDtoRequest>())).ReturnsAsync(new ValidateLoginDtoResponse 
        { 
            Valid = It.IsAny<bool>()
        });
        _mockMapper.Setup(m => m.Map<GenerateTokenJWTDtoRequest>(It.IsAny<LoginApiRequest>())).Returns(new GenerateTokenJWTDtoRequest());
        _mockUserService.Setup(s => s.GenerateTokenJWTAsync(It.IsAny<GenerateTokenJWTDtoRequest>())).ReturnsAsync(new GenerateTokenJWTDtoResponse 
        { 
            Token = It.IsAny<string>()
        });

        var dateNow = DateTime.Now;
        _mockMapper.Setup(m => m.Map<LoginApiResponse>(It.IsAny<GenerateTokenJWTDtoResponse>())).Returns(new LoginApiResponse 
        { 
            Token = "validToken",
            ExpiryDate = dateNow, 
            TokenType = "jwt" 
        });

        // Act
        var result = await _userController.Login
        (
            new()
            { 
                Username = "test@example.com", 
                Password = "password" 
            }
        ) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

        var responseApi = (LoginApiResponse)result.Value;
        Assert.Equal("validToken", responseApi.Token);
        Assert.Equal(dateNow, responseApi.ExpiryDate);
        Assert.Equal("jwt", responseApi.TokenType);
    }

    [Fact]
    public async Task Register_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _userController.Register(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var registerRequest = ;
        
        var registerDtoRequest = new RegisterDtoRequest();
        _mockMapper.Setup(m => m.Map<RegisterDtoRequest>(It.IsAny<RegisterApiRequest>())).Returns(registerDtoRequest);

        _mockUserService.Setup(s => s.RegisterAsync(It.IsAny<RegisterDtoRequest>())).Returns(Task.CompletedTask);

        // Act
        var result = await _userController.Register
        (
            new() 
            { 
                Username = "test@example.com", 
                Password = "password" 
            }
        ) as StatusCodeResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
    }
}

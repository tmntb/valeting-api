using System.Net;
using Api.Controllers;
using Api.Controllers.v1;
using Api.Models.Token.Payload;
using Common.Enums;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Interfaces;
using Service.Models.Auth.Payload;

namespace Api.Tests.Controllers;

public class TokenControllerTests
{
    private readonly Mock<ITokenService> _mockTokenService;

    private readonly TokenController _tokenController;
    
    public TokenControllerTests()
    {
        _mockTokenService = new Mock<ITokenService>();

        _tokenController = new TokenController(_mockTokenService.Object);
    }
    
    [Fact]
    public async Task GenerateJwtTokenAsync_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _tokenController.GenerateJwtTokenAsync(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task GenerateJwtTokenAsync_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var expiryDate = DateTime.UtcNow;
        _mockTokenService
            .Setup(s => s.GenerateTokenJwtAsync(It.IsAny<string>()))
            .ReturnsAsync(
                new GenerateTokenJWTDtoResponse
                {
                    Token = "validToken",
                    TokenType = "jwt",
                    ExpiryDate = expiryDate,
                    Role = RoleEnum.ADMIN
                });
        
        // Act
        var result = await _tokenController.GenerateJwtTokenAsync
        (
            new()
            {
                Email = "test@example.com"
            }
        ) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        
        var responseApi = (JwtTokenApiResponse)result.Value;
        Assert.Equal("validToken", responseApi.Token);
        Assert.Equal(expiryDate, responseApi.ExpiryDate);
        Assert.Equal("jwt", responseApi.TokenType);
        Assert.Equal(RoleEnum.ADMIN, responseApi.Role);
    }
    
    [Fact]
    public async Task RefreshJwtTokenAsync_ShouldThrowArgumentNullException_WhenParamsAreNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _tokenController.RefreshJwtTokenAsync(null));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task RefreshJwtTokenAsync_ShouldThrowArgumentNullException_WhenTokenIsNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _tokenController.RefreshJwtTokenAsync(new() { Token = null }));
        Assert.Contains(Messages.InvalidRequestBody, exception.Message);
    }

    [Fact]
    public async Task RefreshJwtTokenAsync_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        _mockTokenService
            .Setup(s => s.ValidateToken(It.IsAny<string>()))
            .Returns("test@example.com");

        _mockTokenService
            .Setup(s => s.GenerateTokenJwtAsync(It.IsAny<string>()))
            .ReturnsAsync(
                new GenerateTokenJWTDtoResponse
                {
                    Token = "newValidToken",
                    TokenType = "jwt",
                    ExpiryDate = DateTime.UtcNow.AddHours(1),
                    Role = RoleEnum.USER
                });

        // Act
        var result = await _tokenController.RefreshJwtTokenAsync
        (
            new()
            {
                Token = "oldValidToken"
            }
        ) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

        var responseApi = (JwtTokenApiResponse)result.Value;
        Assert.Equal(RoleEnum.USER, responseApi.Role);
    }
}
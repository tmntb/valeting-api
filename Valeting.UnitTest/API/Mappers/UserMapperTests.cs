using AutoMapper;
using Valeting.API.Mappers;
using Valeting.API.Models.User;
using Valeting.Common.Models.User;
using Valeting.Repository.Entities;

namespace Valeting.Tests.API.Mappers;

public class UserMapperTests
{
    private readonly IMapper _mapper;

    public UserMapperTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserMapper>());
        _mapper = config.CreateMapper();
    }

    #region Api -> Dto
    [Fact]
    public void Should_Map_LoginApiRequest_To_ValidateLoginDtoRequest()
    {
        // Arrange
        var source = new LoginApiRequest
        {
            Username = "username",
            Password = "password"
        };

        // Act
        var result = _mapper.Map<ValidateLoginDtoRequest>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Username, result.Username);
        Assert.Equal(source.Password, result.Password);
    }

    [Fact]
    public void Should_Map_LoginApiRequest_To_GenerateTokenJWTDtoRequest()
    {
        // Arrange
        var source = new LoginApiRequest
        {
            Username = "username",
            Password = "password"
        };

        // Act
        var result = _mapper.Map<GenerateTokenJWTDtoRequest>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Username, result.Username);
    }

    [Fact]
    public void Should_Map_RegisterApiRequest_To_RegisterDtoRequest()
    {
        // Arrange
        var source = new RegisterApiRequest
        {
            Username = "username",
            Password = "password"
        };

        // Act
        var result = _mapper.Map<RegisterDtoRequest>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Username, result.Username);
        Assert.Equal(source.Password, result.Password);
    }
    #endregion

    #region Entity -> Dto
    [Fact]
    public void Should_Map_ApplicationUser_To_UserDto()
    {
        // Arrange
        var source = new ApplicationUser
        {
            Username = "username",
            Password = "password"
        };

        // Act
        var result = _mapper.Map<UserDto>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Username, result.Username);
        Assert.Equal(source.Password, result.Password);
    }
    #endregion

    #region Dto -> Entity
    [Fact]
    public void Should_Map_UserDto_To_ApplicationUser()
    {
        // Arrange
        var source = new UserDto
        {
            Username = "username",
            Password = "password"
        };

        // Act
        var result = _mapper.Map<ApplicationUser>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Username, result.Username);
        Assert.Equal(source.Password, result.Password);
    }
    #endregion

    #region Dto -> Api
    [Fact]
    public void Should_Map_GenerateTokenJWTDtoResponse_To_LoginApiResponse()
    {
        // Arrange
        var source = new GenerateTokenJWTDtoResponse
        {
            Token = "token",
            ExpiryDate = DateTime.UtcNow,
            TokenType = "tokenType"
        };

        // Act
        var result = _mapper.Map<LoginApiResponse>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Token, result.Token);
        Assert.Equal(source.ExpiryDate, result.ExpiryDate);
        Assert.Equal(source.TokenType, result.TokenType);
    }
    #endregion
}

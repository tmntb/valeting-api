using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Messages;
using Common.Models.User;
using Service.Interfaces;
using Service.Validators;
using Service.Validators.Utils;

namespace Service.Services;

public class UserService(IUserRepository userRepository, IConfiguration configuration) : IUserService
{
    /// <inheritdoc />
    public async Task<GenerateTokenJWTDtoResponse> GenerateTokenJWTAsync(string username)
    {
        var userDto = await userRepository.GetUserByEmailAsync(username) ?? throw new KeyNotFoundException(Messages.NotFound);

        var secret = configuration["Jwt:Key"];
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim("UserId", userDto.Id.ToString()),
                new Claim("Username", userDto.Username)
            ]),
            Expires = DateTime.Now.AddMinutes(60),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new()
        {
            Token = tokenHandler.WriteToken(token),
            ExpiryDate = token.ValidTo.ToLocalTime(),
            TokenType = tokenHandler.TokenType.Name
        };
    }

    /// <inheritdoc />
    public async Task RegisterAsync(RegisterDtoRequest registerDtoRequest)
    {
        registerDtoRequest.ValidateRequest(new RegisterValidator());

        var userDto = await userRepository.GetUserByEmailAsync(registerDtoRequest.Username);
        if (userDto != null)
        {
            throw new InvalidOperationException(Messages.UsernameInUse);
        }

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDtoRequest.Password, workFactor: 12);

        var registerUserDto = new UserDto
        {
            Id = Guid.NewGuid(),
            Username = registerDtoRequest.Username,
            Password = hashedPassword
        };
        await userRepository.RegisterAsync(registerUserDto);
    }

    /// <inheritdoc />
    public async Task<bool> ValidateLoginAsync(ValidateLoginDtoRequest validateLoginDtoRequest)
    {
        validateLoginDtoRequest.ValidateRequest(new ValidateLoginValidator());

        var userDto = await userRepository.GetUserByEmailAsync(validateLoginDtoRequest.Username) ?? throw new KeyNotFoundException(Messages.NotFound);

        return BCrypt.Net.BCrypt.Verify(validateLoginDtoRequest.Password, userDto.Password);
    }
}
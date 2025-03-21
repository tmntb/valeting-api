using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Valeting.Common.Messages;
using Valeting.Common.Models.User;
using Valeting.Core.Interfaces;
using Valeting.Core.Validators;
using Valeting.Core.Validators.Utils;
using Valeting.Repository.Interfaces;

namespace Valeting.Core.Services;

public class UserService(IUserRepository userRepository, IConfiguration configuration) : IUserService
{
    public async Task<ValidateLoginDtoResponse> ValidateLoginAsync(ValidateLoginDtoRequest validateLoginDtoRequest)
    {
        var validateLoginDtoResponse = new ValidateLoginDtoResponse() { Error = new() };

        validateLoginDtoRequest.ValidateRequest(new ValidateLoginValidator());

        var userDto = await userRepository.GetUserByEmailAsync(validateLoginDtoRequest.Username) ?? throw new KeyNotFoundException(Messages.NotFound);

        validateLoginDtoResponse.Valid = BCrypt.Net.BCrypt.Verify(validateLoginDtoRequest.Password, userDto.Password);
        return validateLoginDtoResponse;
    }

    public async Task<GenerateTokenJWTDtoResponse> GenerateTokenJWTAsync(GenerateTokenJWTDtoRequest generateTokenJWTDtoRequest)
    {
        var generateTokenJWTDtoResponse = new GenerateTokenJWTDtoResponse();

        generateTokenJWTDtoRequest.ValidateRequest(new GenerateTokenJWTValidator());

        var userDto = await userRepository.GetUserByEmailAsync(generateTokenJWTDtoRequest.Username) ?? throw new KeyNotFoundException(Messages.NotFound);

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

        generateTokenJWTDtoResponse.Token = tokenHandler.WriteToken(token);
        generateTokenJWTDtoResponse.ExpiryDate = token.ValidTo.ToLocalTime();
        generateTokenJWTDtoResponse.TokenType = tokenHandler.TokenType.Name;
        return generateTokenJWTDtoResponse;
    }

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
}
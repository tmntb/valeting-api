using Common.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;
using Service.Models.User;
using Service.Models.User.Payload;
using Service.Validators;
using Service.Validators.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Service.Services;

public class UserService(IUserRepository userRepository, IRoleRepository roleRepository, IConfiguration configuration) : IUserService
{
    /// <inheritdoc />
    public async Task<GenerateTokenJWTDtoResponse> GenerateTokenJWTAsync(string username)
    {
        var userDto = await userRepository.GetUserByEmailAsync(username) ?? throw new KeyNotFoundException(Messages.NotFound);

        var (issuer, audience) = GetJwtSettings();

        var securityKey = GetSecurityKey();
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
                new Claim(ClaimTypes.Name, userDto.Username),
                new Claim(ClaimTypes.Role, userDto.Role.Name.ToString())
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

        var roleDto = await roleRepository.GetByNameAsync(registerDtoRequest.RoleName) ?? throw new KeyNotFoundException(Messages.NotFound);
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDtoRequest.Password, workFactor: 12);

        var registerUserDto = new UserDto
        {
            Id = Guid.NewGuid(),
            Username = registerDtoRequest.Username,
            Password = hashedPassword,
            Role = new() { Id = roleDto.Id },
            IsActive = true
        };
        await userRepository.RegisterAsync(registerUserDto);
    }

    /// <inheritdoc />
    public async Task<bool> ValidateLoginAsync(ValidateLoginDtoRequest validateLoginDtoRequest)
    {
        validateLoginDtoRequest.ValidateRequest(new ValidateLoginValidator());

        var userDto = await userRepository.GetUserByEmailAsync(validateLoginDtoRequest.Username) ?? throw new KeyNotFoundException(Messages.NotFound);

        return userDto.IsActive && BCrypt.Net.BCrypt.Verify(validateLoginDtoRequest.Password, userDto.Password);
    }

    /// <inheritdoc />
    public string ValidateToken(string token)
    {
        var (issuer, audience) = GetJwtSettings();
        var securityKey = GetSecurityKey();

        var tokenHandler = new JwtSecurityTokenHandler();
        var claims = tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = securityKey,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = ClaimTypes.Role
        }, out _);

        return claims.FindFirst(ClaimTypes.Name)?.Value ?? throw new UnauthorizedAccessException(Messages.InvalidToken);
    }

    /// <summary>
    /// Retrieves the JWT issuer and audience from configuration.
    /// </summary>
    /// <returns>A tuple containing the issuer and audience values.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the issuer or audience is not configured.</exception>
    private (string Issuer, string Audience) GetJwtSettings()
    {
        var issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer not configured.");
        var audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience not configured.");
        return (issuer, audience);
    }

    /// <summary>
    /// Retrieves the symmetric security key used for JWT signing from configuration.
    /// </summary>
    /// <returns>A <see cref="SymmetricSecurityKey"/> constructed from the configured JWT secret.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the JWT secret is not configured.</exception>
    private SymmetricSecurityKey GetSecurityKey()
    {
        var secret = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT secret not configured.");
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    }
}
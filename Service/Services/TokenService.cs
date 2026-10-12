using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;
using Service.Models.Auth.Payload;

namespace Service.Services;

public class TokenService(IUserRepository userRepository, IConfiguration configuration) : ITokenService
{
     /// <inheritdoc />
    public async Task<GenerateTokenJWTDtoResponse> GenerateTokenJWTAsync(string email)
    {
        var userDto = await userRepository.GetByEmailAsync(email) ?? throw new KeyNotFoundException(Messages.NotFound);

        var (issuer, audience) = GetJwtSettings();

        var securityKey = GetSecurityKey();
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
                new Claim(ClaimTypes.Name, string.Format("{0} {1}", userDto.FirstName, userDto.LastName)),
                new Claim(ClaimTypes.Email, userDto.Email),
                new Claim(ClaimTypes.Role, userDto.Role.Code.ToString())
            ]),
            Expires = DateTime.UtcNow.AddMinutes(60),
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

        return claims.FindFirst(ClaimTypes.Email)?.Value ?? throw new UnauthorizedAccessException(Messages.InvalidToken);
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

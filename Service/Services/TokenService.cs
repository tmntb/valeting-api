using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;
using Service.Models.Token.Payload;
using Service.Validators.Token;
using Service.Validators.Utils;

namespace Service.Services;

public class TokenService(IUserRepository userRepository, IConfiguration configuration) : ITokenService
{
     /// <inheritdoc />
    public async Task<GenerateTokenJwtDtoResponse> GenerateTokenJwtAsync(GenerateTokenJwtDtoRequest generateTokenJwtDtoRequest, CancellationToken cancellationToken = default)
    {
        generateTokenJwtDtoRequest.ValidateRequest(new GenerateTokenJwtValidator());
        
        var userDto = await userRepository.GetByEmailAsync(generateTokenJwtDtoRequest.Email, cancellationToken) ?? throw new KeyNotFoundException(Messages.NotFound);

        var (issuer, audience) = GetJwtSettings();

        var securityKey = GetSecurityKey();
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
            new(ClaimTypes.Name, $"{userDto.FirstName} {userDto.LastName}"),
            new(ClaimTypes.Email, userDto.Email),
            new(ClaimTypes.Role, userDto.Role.Code.ToString())
        };
        
        if (generateTokenJwtDtoRequest.Claims != null)
        {
            claims.AddRange(generateTokenJwtDtoRequest.Claims);
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(generateTokenJwtDtoRequest.Ttl),
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
            TokenType = tokenHandler.TokenType.Name,
            Role = userDto.Role.Code
        };
    }

    /// <inheritdoc />
    public void ValidateTokenJwt(string token)
    {
        var (issuer, audience) = GetJwtSettings();
        var securityKey = GetSecurityKey();

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = securityKey,
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = ClaimTypes.Role
            }, out _);
        }
        catch (SecurityTokenException ex)
        {
            throw new UnauthorizedAccessException(Messages.InvalidToken, ex);
        }
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

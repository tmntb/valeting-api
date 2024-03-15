using System.Net;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

using Valeting.Common.Messages;
using Valeting.Services.Interfaces;
using Valeting.Repositories.Interfaces;
using Valeting.Business.Authentication;

namespace Valeting.Services;

public class AuthenticationService(IUserRepository userRepository, IConfiguration configuration) : IAuthenticationService
{
    public async Task<AuthenticationDTO> GenerateTokenJWT(UserDTO userDTO)
    {
        var authenticationDTO = new AuthenticationDTO(){ Errors = [] };

        var userDTO_DB = await userRepository.FindUserByEmail(userDTO.Username);
        if (userDTO_DB == null)
        {
            authenticationDTO.Errors.Add(new()
            {
                Id = Guid.NewGuid(),
                ErrorCode = (int)HttpStatusCode.NotFound,
                Detail = Messages.UserNotFound
            });

            return authenticationDTO;
        }

        var secret = configuration["Jwt:Key"];
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim("Id", Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, userDTO.Username),
                new Claim(JwtRegisteredClaimNames.Email, userDTO.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ]),
            Expires = DateTime.Now.AddMinutes(60),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        authenticationDTO.Token = tokenHandler.WriteToken(token);
        authenticationDTO.ExpiryDate = token.ValidTo.ToLocalTime();
        authenticationDTO.TokenType = tokenHandler.TokenType.Name;

        return authenticationDTO;
    }
}
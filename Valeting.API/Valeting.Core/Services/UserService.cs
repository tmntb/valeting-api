using System.Net;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

using Valeting.Common.Messages;
using Valeting.Core.Services.Interfaces;
using Valeting.Core.Validators;
using Valeting.Core.Models.User;
using Valeting.Repository.Repositories.Interfaces;

namespace Valeting.Core.Services;

public class UserService(IUserRepository userRepository, IConfiguration configuration) : IUserService
{
    public async Task<ValidateLoginSVResponse> ValidateLogin(ValidateLoginSVRequest validateLoginSVRequest)
    {
        var validateLoginSVResponse = new ValidateLoginSVResponse() { Error = new() };

        var validator = new ValidateLoginValidator();
        var result = validator.Validate(validateLoginSVRequest);
        if(!result.IsValid)
        {
            validateLoginSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return validateLoginSVResponse;
        }

        var userDTO = await userRepository.FindUserByEmail(validateLoginSVRequest.Username);
        if (userDTO == null)
        {
            validateLoginSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.NotFound,
                Message = Messages.UserNotFound
            };
            return validateLoginSVResponse;
        }

        byte[] salt = Encoding.ASCII.GetBytes(userDTO.Salt);

        /*
            * Criar Salt
        byte[] salt = new byte[128 / 8];
        using (RNGCryptoServiceProvider rngCsp = new())
        {
            rngCsp.GetNonZeroBytes(salt);
        }
        */

        var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(validateLoginSVRequest.Password, salt, KeyDerivationPrf.HMACSHA256, 100000, 256 / 8));
        validateLoginSVResponse.Valid = userDTO.Password.Equals(hashed);
        return validateLoginSVResponse;
    }

    public async Task<GenerateTokenJWTSVResponse> GenerateTokenJWT(GenerateTokenJWTSVRequest generateTokenJWTSVRequest)
    {
        var generateTokenJWTSVResponse = new GenerateTokenJWTSVResponse();

        var validator = new GenerateTokenJWTValidator();
        var result = validator.Validate(generateTokenJWTSVRequest);
        if(!result.IsValid)
        {
            generateTokenJWTSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return generateTokenJWTSVResponse;
        }

        var userDTO = await userRepository.FindUserByEmail(generateTokenJWTSVRequest.Username);
        if (userDTO == null)
        {
            generateTokenJWTSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.NotFound,
                Message = Messages.UserNotFound
            };
            return generateTokenJWTSVResponse;
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
                new Claim("UserId", userDTO.Id.ToString()),
                new Claim("Username", userDTO.Username)
            ]),
            Expires = DateTime.Now.AddMinutes(60),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        generateTokenJWTSVResponse.Token = tokenHandler.WriteToken(token);
        generateTokenJWTSVResponse.ExpiryDate = token.ValidTo.ToLocalTime();
        generateTokenJWTSVResponse.TokenType = tokenHandler.TokenType.Name;
        return generateTokenJWTSVResponse;
    }
}
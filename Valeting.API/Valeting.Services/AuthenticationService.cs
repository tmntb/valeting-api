﻿using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using Valeting.Business;
using Valeting.Common.Messages;
using Valeting.Common.Exceptions;
using Valeting.Services.Interfaces;
using Valeting.Repositories.Interfaces;
using Valeting.Business.Authentication;

namespace Valeting.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthenticationService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository ?? throw new Exception(string.Format(Messages.NotInitializeRepository, "User Repository")); ;
            _configuration = configuration;
        }

        public async Task<AuthenticationDTO> GenerateTokenJWT(UserDTO userDTO)
        {
            var userDTO_DB = await _userRepository.FindUserByEmail(userDTO.Username);
            if (userDTO_DB == null)
                throw new NotFoundException(Messages.UserNotFound);

            var secret = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, userDTO.Username),
                    new Claim(JwtRegisteredClaimNames.Email, userDTO.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.Now.AddMinutes(60),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var authenticationDTO = new AuthenticationDTO()
            {
                Token = tokenHandler.WriteToken(token),
                ExpiryDate = token.ValidTo.ToLocalTime(),
                TokenType = tokenHandler.TokenType.Name
            };
            
            return authenticationDTO;
        }
    }
}


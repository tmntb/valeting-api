using Api.Controllers.v1.BaseController;
using Api.Models.Token.Payload;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Api.Controllers.v1;

/// <summary>
/// Handles JWT generation and refresh requests.
/// </summary>
/// <param name="tokenService">JWT generation and validation operations.</param>
public class TokenController(ITokenService tokenService) : TokenBaseController
{
    /// <inheritdoc />
    public override async Task<IActionResult> GenerateJwtTokenAsync(GenerateJwtTokenApiRequest generateJwtTokenApiRequest)
    {
        ArgumentNullException.ThrowIfNull(generateJwtTokenApiRequest, Messages.InvalidRequestBody);
        
        var generateTokenJwtDtoResponse = await tokenService.GenerateTokenJwtAsync(generateJwtTokenApiRequest.Email);

        var jwtTokenApiResponse = new JwtTokenApiResponse
        {
            Token = generateTokenJwtDtoResponse.Token,
            TokenType = generateTokenJwtDtoResponse.TokenType,
            ExpiryDate = generateTokenJwtDtoResponse.ExpiryDate,
            Role = generateTokenJwtDtoResponse.Role
        };
        return Ok(jwtTokenApiResponse);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> RefreshJwtTokenAsync([FromBody] RefreshJwtTokenApiRequest refreshJwtTokenApiRequest)
    {
        ArgumentNullException.ThrowIfNull(refreshJwtTokenApiRequest, Messages.InvalidRequestBody);
        ArgumentException.ThrowIfNullOrEmpty(refreshJwtTokenApiRequest.Token, Messages.InvalidRequestBody);

        var email = tokenService.ValidateToken(refreshJwtTokenApiRequest.Token); // TODO: Remove when middleware is implemented
        var generateTokenJwtDtoResponse = await tokenService.GenerateTokenJwtAsync(email);

        var jwtTokenApiResponse = new JwtTokenApiResponse
        {
            Token = generateTokenJwtDtoResponse.Token,
            TokenType = generateTokenJwtDtoResponse.TokenType,
            ExpiryDate = generateTokenJwtDtoResponse.ExpiryDate,
            Role = generateTokenJwtDtoResponse.Role
        };
        return Ok(jwtTokenApiResponse);
    }
}
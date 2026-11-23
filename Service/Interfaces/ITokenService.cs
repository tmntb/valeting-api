using Service.Models.Auth.Payload;
using Service.Models.TokenService.Payload;

namespace Service.Interfaces;

/// <summary>
/// Defines JWT generation and validation operations.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT access token for the specified user.
    /// </summary>
    /// <param name="email">The email of the user for whom the token is generated.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="GenerateTokenJwtDtoResponse"/> 
    /// with the token, its type, and expiration date.
    /// </returns>
    Task<GenerateTokenJwtDtoResponse> GenerateTokenJwtAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a JWT token.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    void ValidateToken(string token);
}

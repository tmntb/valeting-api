using Service.Models.Auth.Payload;

namespace Service.Interfaces;

public interface ITokenService
{
    /// <summary>
    /// Generates a JWT access token for the specified user.
    /// </summary>
    /// <param name="email">The email of the user for whom the token is generated.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="GenerateTokenJWTDtoResponse"/> 
    /// with the token, its type, and expiration date.
    /// </returns>
    Task<GenerateTokenJWTDtoResponse> GenerateTokenJwtAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a JWT token and extracts the username claim.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <returns>The email extracted from the token's claims.</returns>
    string ValidateToken(string token);
}

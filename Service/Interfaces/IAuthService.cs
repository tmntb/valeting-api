using Service.Models.Auth.Payload;
using Service.Models.User;

namespace Service.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Restores the password for a user by their email.
    /// </summary>
    /// <param name="forgotPasswordDtoRequest">The forgo password payload.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ForgotPasswordAsync(ForgotPasswordDtoRequest forgotPasswordDtoRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new user with the provided username and password.
    /// </summary>
    /// <param name="userDto">The user registration information.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RegisterAsync(UserDto userDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates the user's credentials by checking the username and password.
    /// </summary>
    /// <param name="userDto">The user information.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>True if the username exists and the password matches; otherwise, false.</returns>
    Task ValidateLoginAsync(UserDto userDto, CancellationToken cancellationToken = default);
}

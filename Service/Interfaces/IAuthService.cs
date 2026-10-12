using Service.Models.Auth.Payload;
using Service.Models.User;

namespace Service.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Initiates the forgot password process for a user by their email.
    /// </summary>
    /// <param name="email">The email of the user who forgot their password.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ForgotPasswordAsync(ForgotPasswordDtoRequest forgotPasswordDtoRequest);

    /// <summary>
    /// Registers a new user with the provided username and password.
    /// </summary>
    /// <param name="userDto">The user registration information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the username is already in use.</exception>
    Task RegisterAsync(UserDto userDto);

    /// <summary>
    /// Validates the user's credentials by checking the username and password.
    /// </summary>
    /// <param name="userDto">The user information.</param>
    /// <returns>True if the username exists and the password matches; otherwise, false.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if no user is found with the given username.</exception>
    Task ValidateLoginAsync(UserDto userDto);
}

using Service.Models.User;

namespace Service.Interfaces;

public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their email/username from the database.
    /// </summary>
    /// <param name="username">The email or username of the user to retrieve.</param>
    /// <returns>A task that returns a <see cref="UserDto"/> if found; otherwise, null.</returns>
    Task<UserDto> GetUserByEmailAsync(string email);

    /// <summary>
    /// Registers a new user in the database.
    /// </summary>
    /// <param name="userDto">The user data to be registered.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RegisterAsync(UserDto userDto);

    /// <summary>
    /// Updates an existing user's information in the database.
    /// </summary>
    /// <param name="userDto">The user data to be updated.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(UserDto userDto);
}
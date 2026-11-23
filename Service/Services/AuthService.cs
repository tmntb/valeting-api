using Common.Messages;
using Service.Helpers;
using Service.Interfaces;
using Service.Models.Auth.Payload;
using Service.Models.User;
using Service.Validators.Auth;
using Service.Validators.Utils;

namespace Service.Services;

public class AuthService(IUserRepository userRepository, IRoleRepository roleRepository) : IAuthService
{
    /// <inheritdoc />
    public async Task ForgotPasswordAsync(ForgotPasswordDtoRequest forgotPasswordDtoRequest, CancellationToken cancellationToken = default)
    {
        forgotPasswordDtoRequest.ValidateRequest(new ForgotPasswordValidator());

        var userDto = await userRepository.GetByEmailAsync(forgotPasswordDtoRequest.Email, cancellationToken);
        if (userDto == null)
        {
            // Avoid password reset probing.
            return;
        }

        userDto.PasswordHash = HashHelper.GenerateHash(forgotPasswordDtoRequest.NewPassword);
        await userRepository.UpdatePasswordAsync(userDto, cancellationToken);
    }

    /// <inheritdoc />
    public async Task RegisterAsync(UserDto userDto, CancellationToken cancellationToken = default)
    {
        userDto.ValidateRequest(new RegisterValidator());

        var userDtoCheck = await userRepository.GetByEmailAsync(userDto.Email, cancellationToken);
        if (userDtoCheck != null)
        {
            throw new InvalidOperationException(Messages.EmailInUse);
        }

        var roleDto = await roleRepository.GetByCodeAsync(userDto.Role.Code, cancellationToken) ?? throw new KeyNotFoundException(Messages.NotFound);
        var hashedPassword = HashHelper.GenerateHash(userDto.Password);

        userDto.Id = Guid.NewGuid();
        userDto.PasswordHash = hashedPassword;
        userDto.Password = null;
        userDto.Role = new() { Id = roleDto.Id };
        userDto.IsActive = true;
        userDto.TwoFactorAuthEnabled = false;
        userDto.CreatedAt = DateTime.UtcNow;

        await userRepository.RegisterAsync(userDto, cancellationToken);
    }

    /// <inheritdoc />
    public async Task ValidateLoginAsync(UserDto userDto, CancellationToken cancellationToken = default)
    {
        userDto.ValidateRequest(new ValidateLoginValidator());

        var userDtoCheck = await userRepository.GetByEmailAsync(userDto.Email, cancellationToken) ?? throw new KeyNotFoundException(Messages.NotFound);

        var passwordValid = userDtoCheck.IsActive && BCrypt.Net.BCrypt.Verify(userDto.Password, userDtoCheck.PasswordHash);
        if (!passwordValid)
            throw new UnauthorizedAccessException(Messages.InvalidPassword);

        await userRepository.UpdateLastLoginAsync(userDtoCheck.Id, cancellationToken);
    }
}

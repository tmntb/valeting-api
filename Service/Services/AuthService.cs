using Common.Messages;
using Service.Helpers;
using Service.Interfaces;
using Service.Models.Auth.Payload;
using Service.Models.User;
using Service.Validators.Auth;
using Service.Validators.Utils;

namespace Service.Services;

public class AuthService(IUserRepository userRepository, IRecoveryCodeRepository recoveryCodeRepository, IRoleRepository roleRepository) : IAuthService
{
    /// <inheritdoc />
    public async Task ForgotPasswordAsync(ForgotPasswordDtoRequest forgotPasswordDtoRequest)
    {
        forgotPasswordDtoRequest.ValidateRequest(new ForgotPasswordValidator());

        var userDto = await userRepository.GetByEmailAsync(forgotPasswordDtoRequest.Email);
        if (userDto == null)
        {
            // Avoid password reset probing.
            return;
        }

        if (forgotPasswordDtoRequest.MfaCode != null)
        {
            MfaHelper.VerifyMfaCode(userDto, forgotPasswordDtoRequest.MfaCode);
        }

        if (forgotPasswordDtoRequest.RecoveryCode != null)
        {
            var userRecoveryCodes = await recoveryCodeRepository.GetUserRecoveryCodesAsync(userDto.Id);
            if (userRecoveryCodes == null || !userRecoveryCodes.Any())
            {
                throw new InvalidOperationException(Messages.NoRecoveryCodesForUser);
            }

            var recoveryCode = userRecoveryCodes.SingleOrDefault(rc => rc.UsedAt == null && BCrypt.Net.BCrypt.Verify(forgotPasswordDtoRequest.RecoveryCode, rc.CodeHash))
                ?? throw new InvalidOperationException(Messages.InvalidRecoveryCode);

            await recoveryCodeRepository.UpdateUsedAtAsync(recoveryCode.Id);
        }

        userDto.PasswordHash = HashHelper.GenerateHash(forgotPasswordDtoRequest.NewPassword);
        await userRepository.UpdatePasswordAsync(userDto);
    }

    /// <inheritdoc />
    public async Task RegisterAsync(UserDto userDto)
    {
        userDto.ValidateRequest(new RegisterValidator());

        var userDtoCheck = await userRepository.GetByEmailAsync(userDto.Email);
        if (userDtoCheck != null)
        {
            throw new InvalidOperationException(Messages.EmailInUse);
        }

        var roleDto = await roleRepository.GetByCodeAsync(userDto.Role.Code) ?? throw new KeyNotFoundException(Messages.NotFound);
        var hashedPassword = HashHelper.GenerateHash(userDto.Password);

        userDto.Id = Guid.NewGuid();
        userDto.PasswordHash = hashedPassword;
        userDto.Password = null;
        userDto.Role = new() { Id = roleDto.Id };
        userDto.IsActive = true;
        userDto.MfaEnabled = false;
        userDto.CreatedAt = DateTime.UtcNow;

        await userRepository.RegisterAsync(userDto);
    }

    /// <inheritdoc />
    public async Task ValidateLoginAsync(UserDto userDto)
    {
        userDto.ValidateRequest(new ValidateLoginValidator());

        var userDtoCheck = await userRepository.GetByEmailAsync(userDto.Email) ?? throw new KeyNotFoundException(Messages.NotFound);

        var passwordValid = userDtoCheck.IsActive && BCrypt.Net.BCrypt.Verify(userDto.Password, userDtoCheck.PasswordHash);
        if (!passwordValid)
            throw new UnauthorizedAccessException(Messages.InvalidPassword);

        await userRepository.UpdateLastLoginAsync(userDtoCheck.Id);
    }
}

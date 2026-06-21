using Common.Messages;
using Service.Interfaces;
using Service.Models.User;
using Service.Models.User.Payload;
using Service.Validators.User;
using Service.Validators.Utils;

namespace Service.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    /// <inheritdoc />
    public async Task ForgotPasswordAsync(string email)
    {
        var userDto = await userRepository.GetByEmailAsync(email);
        if (userDto == null)
        {
            // To prevent user enumeration, we return a success response even if the email does not exist.
            return;
        }

        var code = Random.Shared.Next(100000, 999999).ToString();

        var hash = GenerateHash(code);
        // userDto.PasswordResetTokenHash = hash;
        // userDto.PasswordResetExpiresAt = DateTime.UtcNow.AddMinutes(10);

        await userRepository.UpdateResetPasswordTokenAsync(userDto);

        // await emailService.SendResetPasswordCodeAsync(email, code);
    }

    

    /// <inheritdoc />
    public async Task ResetAsync(UserDto userDto)
    {
        userDto.ValidateRequest(new ResetValidator());

        var userDtoReset = await userRepository.GetByEmailAsync(userDto.Email) ?? throw new KeyNotFoundException(Messages.NotFound);

        userDtoReset.PasswordHash = GenerateHashPassword(userDto.Password);
        await userRepository.UpdatePasswordAsync(userDtoReset);
    }

    /// <inheritdoc />
    public async Task UpdateAdminSettingsAsync(UpdateAdminSettingsDtoRequest updateAdminSettingsDtoRequest)
    {
        updateAdminSettingsDtoRequest.ValidateRequest(new UpdateAdminSettingsValidator());

        var userDtoUpdate = await userRepository.GetByIdAsync(updateAdminSettingsDtoRequest.UserId) ?? throw new KeyNotFoundException(Messages.NotFound);

        userDtoUpdate.IsActive = updateAdminSettingsDtoRequest.IsActive ?? userDtoUpdate.IsActive;
        userDtoUpdate.Role.Id = updateAdminSettingsDtoRequest.RoleId ?? userDtoUpdate.Role.Id;
        await userRepository.UpdateAdminSettingsAsync(userDtoUpdate);
    }

    /// <inheritdoc />
    public async Task UpdateEmailAsync(UserDto userDto)
    {
        userDto.ValidateRequest(new UpdateEmailValidator());

        var userDtoCheck = await userRepository.GetByIdAsync(userDto.Id) ?? throw new KeyNotFoundException(Messages.NotFound);
        if (userDtoCheck.Email.Equals(userDto.Email, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(Messages.SameEmailInUse);
        }

        var userDtoEmailCheck = await userRepository.GetByEmailAsync(userDto.Email);
        if (userDtoEmailCheck != null)
        {
            throw new InvalidOperationException(Messages.EmailInUse);
        }

        userDtoCheck.Email = userDto.Email;
        await userRepository.UpdateEmailAsync(userDtoCheck);
    }

    /// <inheritdoc />
    public async Task UpdatePasswordAsync(UserDto userDto)
    {
        userDto.ValidateRequest(new UpdatePasswordValidator());

        var userDtoToUpdate = await userRepository.GetByIdAsync(userDto.Id) ?? throw new KeyNotFoundException(Messages.NotFound);

        userDtoToUpdate.PasswordHash = GenerateHashPassword(userDto.Password);
        await userRepository.UpdatePasswordAsync(userDtoToUpdate);
    }

    /// <inheritdoc />
    public async Task UpdateProfileAsync(UpdateProfileDtoRequest updateProfileDtoRequest)
    {
        updateProfileDtoRequest.ValidateRequest(new UpdateProfileValidator());

        var userDtoUpdate = await userRepository.GetByIdAsync(updateProfileDtoRequest.Id) ?? throw new KeyNotFoundException(Messages.NotFound);

        userDtoUpdate.FirstName = updateProfileDtoRequest.FirstName ?? userDtoUpdate.FirstName;
        userDtoUpdate.LastName = updateProfileDtoRequest.LastName ?? userDtoUpdate.LastName;
        userDtoUpdate.DateOfBirth = updateProfileDtoRequest.DateOfBirth ?? userDtoUpdate.DateOfBirth;
        userDtoUpdate.ContactNumber = updateProfileDtoRequest.ContactNumber ?? userDtoUpdate.ContactNumber;

        await userRepository.UpdateProfileAsync(userDtoUpdate);
    }

    /// <summary>
    /// Generates a hashed string using BCrypt with a specified work factor.
    /// </summary>
    /// <param name="strToHash">The plain text string to hash.</param>
    /// <returns>A hashed version of the string.</returns>
    private string GenerateHash(string strToHash)
    {
        return BCrypt.Net.BCrypt.HashPassword(strToHash, workFactor: 12);
    }
}
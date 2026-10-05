using Common.Messages;
using Service.Helpers;
using Service.Interfaces;
using Service.Models.User;
using Service.Models.User.Payload;
using Service.Validators.User;
using Service.Validators.Utils;

namespace Service.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
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

        userDtoToUpdate.PasswordHash = HashHelper.GenerateHash(userDto.Password);
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
}
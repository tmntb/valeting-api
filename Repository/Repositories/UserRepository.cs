using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Service.Interfaces;
using Service.Models.User;

namespace Repository.Repositories;

public class UserRepository(ValetingContext valetingContext) : IUserRepository
{
    /// <inheritdoc />
    public async Task<UserDto> GetByEmailAsync(string email)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == email);

        return CreateUserDto(applicationUser);
    }

    /// <inheritdoc />
    public async Task<UserDto> GetByIdAsync(Guid id)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == id);

        return CreateUserDto(applicationUser);
    }
    /// <inheritdoc />
    public async Task RegisterAsync(UserDto userDto)
    {
        var applicationUser = new ApplicationUser
        {
            Id = userDto.Id,
            Email = userDto.Email,
            PasswordHash = userDto.PasswordHash,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            DateOfBirth = userDto.DateOfBirth,
            ContactNumber = userDto.ContactNumber,
            RoleId = userDto.Role.Id,
            IsActive = userDto.IsActive,
            MfaEnabled = userDto.MfaEnabled,
            MfaSecret = userDto.MfaSecret,
            CreatedAt = userDto.CreatedAt,
            UpdatedAt = userDto.UpdatedAt,
            LastLoginAt = userDto.LastLoginAt
        };
        await valetingContext.ApplicationUsers.AddAsync(applicationUser);
        await valetingContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateAdminSettingsAsync(UserDto userDto)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userDto.Id);
        if (applicationUser == null)
            return;

        applicationUser.UpdateAdminSettings(userDto.Role.Id, userDto.IsActive);

        await valetingContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateEmailAsync(UserDto userDto)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userDto.Id);
        if (applicationUser == null)
            return;

        applicationUser.UpdateEmail(userDto.Email);

        await valetingContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateLastLoginAsync(Guid id)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == id);
        if (applicationUser == null)
            return;

        applicationUser.UpdateLastLogin();

        await valetingContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateMfaSecretAsync(UserDto userDto)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userDto.Id);
        if (applicationUser == null)
            return;

        applicationUser.UpdateMfaSecret(userDto.MfaSecret);

        await valetingContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdatePasswordAsync(UserDto userDto)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userDto.Id);
        if (applicationUser == null)
            return;

        applicationUser.UpdatePassword(userDto.PasswordHash);

        await valetingContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateProfileAsync(UserDto userDto)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userDto.Id);
        if (applicationUser == null)
            return;

        applicationUser.UpdateProfile(userDto.FirstName, userDto.LastName, userDto.DateOfBirth, userDto.ContactNumber);

        await valetingContext.SaveChangesAsync();
    }

    /// <summary>
    /// Creates a <see cref="UserDto"/> from an <see cref="ApplicationUser"/>.
    /// </summary>
    /// <param name="applicationUser">The application user entity.</param>
    /// <returns>A <see cref="UserDto"/> representing the application user.</returns>
    private static UserDto CreateUserDto(ApplicationUser? applicationUser)
    {
        if (applicationUser == null)
            return null;

        return new()
        {
            Id = applicationUser.Id,
            Email = applicationUser.Email,
            PasswordHash = applicationUser.PasswordHash,
            FirstName = applicationUser.FirstName,
            LastName = applicationUser.LastName,
            DateOfBirth = applicationUser.DateOfBirth,
            ContactNumber = applicationUser.ContactNumber,
            Role = new()
            {
                Id = applicationUser.Role.Id,
                Code = applicationUser.Role.Code
            },
            IsActive = applicationUser.IsActive,
            MfaEnabled = applicationUser.MfaEnabled,
            MfaSecret = applicationUser.MfaSecret,
            CreatedAt = applicationUser.CreatedAt,
            UpdatedAt = applicationUser.UpdatedAt,
            LastLoginAt = applicationUser.LastLoginAt
        };
    }
}
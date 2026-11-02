using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Service.Interfaces;
using Service.Models.User;

namespace Repository.Repositories;

public class UserRepository(ValetingContext valetingContext) : IUserRepository
{
    /// <inheritdoc />
    public async Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        return ApplicationUser.FromEntity(applicationUser);
    }

    /// <inheritdoc />
    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return ApplicationUser.FromEntity(applicationUser);
    }
    /// <inheritdoc />
    public async Task RegisterAsync(UserDto userDto, CancellationToken cancellationToken = default)
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
        await valetingContext.ApplicationUsers.AddAsync(applicationUser, cancellationToken);
        await valetingContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAdminSettingsAsync(UserDto userDto, CancellationToken cancellationToken = default)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userDto.Id, cancellationToken);
        if (applicationUser == null)
            return;

        applicationUser.UpdateAdminSettings(userDto.Role.Id, userDto.IsActive);

        await valetingContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateEmailAsync(UserDto userDto, CancellationToken cancellationToken = default)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userDto.Id, cancellationToken);
        if (applicationUser == null)
            return;

        applicationUser.UpdateEmail(userDto.Email);

        await valetingContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateLastLoginAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (applicationUser == null)
            return;

        applicationUser.UpdateLastLogin();

        await valetingContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateMfaEnableAsync(UserDto userDto, CancellationToken cancellationToken = default)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userDto.Id, cancellationToken);
        if (applicationUser == null)
            return;

        applicationUser.UpdateMfaEnable(userDto.MfaEnabled);

        await valetingContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateMfaSecretAsync(UserDto userDto, CancellationToken cancellationToken = default)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userDto.Id, cancellationToken);
        if (applicationUser == null)
            return;

        applicationUser.UpdateMfaSecret(userDto.MfaSecret);

        await valetingContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdatePasswordAsync(UserDto userDto, CancellationToken cancellationToken = default)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userDto.Id, cancellationToken);
        if (applicationUser == null)
            return;

        applicationUser.UpdatePassword(userDto.PasswordHash);

        await valetingContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateProfileAsync(UserDto userDto, CancellationToken cancellationToken = default)
    {
        var applicationUser = await valetingContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userDto.Id, cancellationToken);
        if (applicationUser == null)
            return;

        applicationUser.UpdateProfile(userDto.FirstName, userDto.LastName, userDto.DateOfBirth, userDto.ContactNumber);

        await valetingContext.SaveChangesAsync(cancellationToken);
    }
}
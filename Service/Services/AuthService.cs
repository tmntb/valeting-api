using Common.Messages;
using OtpNet;
using Service.Interfaces;
using Service.Models.Auth;
using Service.Models.Auth.Payload;
using Service.Models.User;
using Service.Validators.Auth;
using Service.Validators.Utils;

namespace Service.Services;

public class AuthService(IUserRepository userRepository, IRecoveryCodeRepository recoveryCodeRepository, IRoleRepository roleRepository) : IAuthService
{
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
        var hashedPassword = GenerateHash(userDto.Password);

        userDto.Id = Guid.NewGuid();
        userDto.PasswordHash = hashedPassword;
        userDto.Password = null;
        userDto.Role = new() { Id = roleDto.Id };
        userDto.IsActive = true;
        userDto.MfaEnabled = false;
        userDto.CreatedAt = DateTime.UtcNow;

        await userRepository.RegisterAsync(userDto);
    }

    public async Task<MfaSetupDtoResponse> MfaSetupAsync()
    {
        var createDate = DateTime.UtcNow;

        var mfaSecret = GenerateMfaSecret();
        var recoveryCodes = GenerateRecoveryCodes();

        recoveryCodes.ForEach(async x => await recoveryCodeRepository.CreateAsync(new RecoveryCodeDto
        {
            Id = Guid.NewGuid(),
            CodeHash = GenerateHash(x),
            CreatedAt = createDate,
            User = new UserDto
            {
                //Id = userId
            }
        }));

        return new()
        {
            //MfaQrCodeUri = $"otpauth://totp/Valeting:{Uri.EscapeDataString(userDto.Email)}?secret={mfaSecret}&issuer=Valeting",
            RecoveryCodes = recoveryCodes
        };
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

    /// <summary>
    /// Generates a secret for multi-factor authentication.
    /// </summary>
    /// <returns>A base32-encoded secret.</returns>
    private string GenerateMfaSecret()
    {
        var key = KeyGeneration.GenerateRandomKey(20);
        return Base32Encoding.ToString(key);
    }

    /// <summary>
    /// Generates a list of recovery codes for multi-factor authentication, each consisting of two sets of four digits separated by a hyphen.
    /// </summary>
    /// <returns>A list of 8 recovery codes.</returns>
    private List<string> GenerateRecoveryCodes()
    {
        return Enumerable.Range(0, 8)
                .Select(_ => GenerateCode())
                .ToList();
    }

    /// <summary>
    /// Generates a single recovery code consisting of two sets of four digits separated by a hyphen (e.g., "1234-5678").
    /// </summary>
    /// <returns>A recovery code.</returns>
    private string GenerateCode()
    {
        return $"{Random.Shared.Next(1000, 9999)}-{Random.Shared.Next(1000, 9999)}";
    }


}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OtpNet;
using Service.Helpers;
using Service.Interfaces;
using Service.Models.Auth;
using Service.Models.Auth.Payload;
using Service.Models.User;
using Service.Validators.Auth;
using Service.Validators.Utils;

namespace Service.Services;

public class AuthService(IUserRepository userRepository, IRecoveryCodeRepository recoveryCodeRepository, IRoleRepository roleRepository, IConfiguration configuration) : IAuthService
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
            ValidateMfaCode(userDto, forgotPasswordDtoRequest.MfaCode);
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
    public async Task<GenerateTokenJWTDtoResponse> GenerateTokenJWTAsync(string email)
    {
        var userDto = await userRepository.GetByEmailAsync(email) ?? throw new KeyNotFoundException(Messages.NotFound);

        var (issuer, audience) = GetJwtSettings();

        var securityKey = GetSecurityKey();
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
                new Claim(ClaimTypes.Name, string.Format("{0} {1}", userDto.FirstName, userDto.LastName)),
                new Claim(ClaimTypes.Email, userDto.Email),
                new Claim(ClaimTypes.Role, userDto.Role.Code.ToString())
            ]),
            Expires = DateTime.UtcNow.AddMinutes(60),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new()
        {
            Token = tokenHandler.WriteToken(token),
            ExpiryDate = token.ValidTo.ToLocalTime(),
            TokenType = tokenHandler.TokenType.Name
        };
    }

    /// <inheritdoc />
    public async Task MfaEnableAsync(MfaCodeDtoRequest mfaCodeDtoRequest)
    {
        mfaCodeDtoRequest.ValidateRequest(new MfaCodeValidator());

        var userDto = await userRepository.GetByIdAsync(mfaCodeDtoRequest.UserId) ?? throw new KeyNotFoundException(Messages.NotFound);
        if (userDto.MfaEnabled)
        {
            throw new InvalidOperationException(Messages.MfaActivated);
        }

        ValidateMfaCode(userDto, mfaCodeDtoRequest.MfaCode);

        userDto.MfaEnabled = true;
        await userRepository.UpdateMfaEnableAsync(userDto);
    }

    /// <inheritdoc />
    public async Task<MfaSetupDtoResponse> MfaSetupAsync(Guid userId)
    {
        var userDto = await userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException(Messages.NotFound);
        if (userDto.MfaEnabled)
        {
            throw new InvalidOperationException(Messages.MfaActivated);
        }

        if (userDto.MfaSecret != null && !userDto.MfaEnabled)
        {
            return new()
            {
                MfaQrCodeUri = $"otpauth://totp/Valeting:{Uri.EscapeDataString(userDto.Email)}?secret={userDto.MfaSecret}&issuer=Valeting"
            };
        }

        var mfaSecret = GenerateMfaSecret();
        userDto.MfaSecret = mfaSecret;
        await userRepository.UpdateMfaSecretAsync(userDto);

        return new()
        {
            MfaQrCodeUri = $"otpauth://totp/Valeting:{Uri.EscapeDataString(userDto.Email)}?secret={mfaSecret}&issuer=Valeting"
        };
    }

    /// <inheritdoc />
    public async Task<List<string>> RecoveryCodesGenerateAsync(Guid userId)
    {
        var userDto = await userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException(Messages.NotFound);
        if (userDto.MfaSecret == null || !userDto.MfaEnabled)
        {
            throw new InvalidOperationException(Messages.MfaDisabled);
        }

        await recoveryCodeRepository.DeleteManyAsync(userDto.Id);

        var createDate = DateTime.UtcNow;

        var recoveryCodes = Enumerable.Range(0, 8)
                .Select(_ => GenerateCode())
                .ToList();

        var recoveryCodeDtos = recoveryCodes.Select(code => new RecoveryCodeDto
        {
            Id = Guid.NewGuid(),
            User = new() { Id = userDto.Id },
            CodeHash = HashHelper.GenerateHash(code),
            CreatedAt = createDate
        });

        await recoveryCodeRepository.CreateManyAsync(recoveryCodeDtos);

        return recoveryCodes;
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

    /// <inheritdoc />
    public string ValidateToken(string token)
    {
        var (issuer, audience) = GetJwtSettings();
        var securityKey = GetSecurityKey();

        var tokenHandler = new JwtSecurityTokenHandler();
        var claims = tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = securityKey,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = ClaimTypes.Role
        }, out _);

        return claims.FindFirst(ClaimTypes.Email)?.Value ?? throw new UnauthorizedAccessException(Messages.InvalidToken);
    }

    /// <summary>
    /// Generates a single recovery code consisting of two sets of four digits separated by a hyphen (e.g., "1234-5678").
    /// </summary>
    /// <returns>A recovery code.</returns>
    private string GenerateCode()
    {
        return $"{Random.Shared.Next(1000, 9999)}-{Random.Shared.Next(1000, 9999)}";
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
    /// Retrieves the JWT issuer and audience from configuration.
    /// </summary>
    /// <returns>A tuple containing the issuer and audience values.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the issuer or audience is not configured.</exception>
    private (string Issuer, string Audience) GetJwtSettings()
    {
        var issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer not configured.");
        var audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience not configured.");
        return (issuer, audience);
    }

    /// <summary>
    /// Retrieves the symmetric security key used for JWT signing from configuration.
    /// </summary>
    /// <returns>A <see cref="SymmetricSecurityKey"/> constructed from the configured JWT secret.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the JWT secret is not configured.</exception>
    private SymmetricSecurityKey GetSecurityKey()
    {
        var secret = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT secret not configured.");
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    }

    /// <summary>
    /// Validates the mfa code sent in the request
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the user mfa code is invalid.</exception>
    private void ValidateMfaCode(UserDto userDto, string mfaCode)
    {
        var secretBytes = Base32Encoding.ToBytes(userDto.MfaSecret);
        var totp = new Totp(secretBytes);
        var isValid = totp.VerifyTotp(mfaCode, out _, new VerificationWindow(previous: 1, future: 1));
        if (!isValid)
        {
            throw new InvalidOperationException(Messages.InvalidMfaCode);
        }
    }
}

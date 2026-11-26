using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using Common.Cache.Interfaces;
using Common.Configs;
using Common.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Service.Helpers;
using Service.Interfaces;
using Service.Models.Auth.Payload;
using Service.Models.Token.Payload;
using Service.Models.User;
using Service.Validators.Auth;
using Service.Validators.Utils;

namespace Service.Services;

public class AuthService(IEmailService emailService, ITokenService tokenService, 
    IUserRepository userRepository, IRoleRepository roleRepository, 
    ICacheHandler cacheHandler, IOptions<PasswordResetConfig> passwordResetConfig, ILogger<AuthService> logger) : IAuthService
{
    private readonly PasswordResetConfig _passwordResetConfig = passwordResetConfig.Value;
    
    /// <inheritdoc />
    public async Task ForgotPasswordAsync(ForgotPasswordDtoRequest forgotPasswordDtoRequest, CancellationToken cancellationToken = default)
    {
        forgotPasswordDtoRequest.ValidateRequest(new ForgotPasswordValidator());

        var userDto = await userRepository.GetByEmailAsync(forgotPasswordDtoRequest.Email, cancellationToken);
        if (userDto == null)
            return;  // Avoid password reset probing.

        var generateTokenJwtDtoResponse = await cacheHandler.GetOrCreateRecordAsync(
            userDto.Id, 
            async () =>
            {
                var jti = Guid.NewGuid().ToString("N");

                return await tokenService.GenerateTokenJwtAsync(new()
                {
                    Email = userDto.Email,
                    Ttl = _passwordResetConfig.LifetimeMinutes,
                    Claims =
                    [
                        new Claim("purpose", "password-reset"),
                        new Claim(JwtRegisteredClaimNames.Jti, jti)
                    ]
                }, cancellationToken);
            }, 
            new()
            {
                AbsoluteExpireTime = TimeSpan.FromMinutes(_passwordResetConfig.LifetimeMinutes)
            },cancellationToken);
        
        var baseUrl = _passwordResetConfig.BaseUrl;
        var separator = baseUrl.Contains('?') ? "&" : "?";
        var resetLink = $"{baseUrl.TrimEnd('/')}{separator}token={Uri.EscapeDataString(generateTokenJwtDtoResponse.Token)}";

        try
        {
            await emailService.SendAsync(new()
            {
                To = userDto.Email,
                Subject = "Valeting password reset",
                Body = $"Reset your password using this link: <a href=\"{resetLink}\">{resetLink}</a>"
            }, cancellationToken);
        }
        catch (SmtpException ex)
        {
            logger.LogWarning(ex, "Failed to send password reset email.");
        }
    }

    /// <inheritdoc />
    public async Task ResetPasswordAsync(ResetPasswordDtoRequest resetPasswordDtoRequest, CancellationToken cancellationToken = default)
    {
        resetPasswordDtoRequest.ValidateRequest(new ResetPasswordValidator());
        
        tokenService.ValidateTokenJwt(resetPasswordDtoRequest.Token);
        
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(resetPasswordDtoRequest.Token);
        var userIdClaim = jwt.Claims
            .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.NameId)?.Value;
        
        var userId = Guid.Parse(userIdClaim);
        
        var generateTokenJwtDtoResponse = await cacheHandler.GetOrCreateRecordAsync(
            userId, 
            () => Task.FromResult<GenerateTokenJwtDtoResponse?>(null),
            new(),
            cancellationToken);

        if (generateTokenJwtDtoResponse == null)
            return;
        
        var userDto = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (userDto == null)
           return;

        userDto.PasswordHash = HashHelper.GenerateHash(resetPasswordDtoRequest.NewPassword);
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

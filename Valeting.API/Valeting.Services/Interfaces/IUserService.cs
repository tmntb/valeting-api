using Valeting.Services.Objects.User;

namespace Valeting.Services.Interfaces;

public interface IUserService
{
    Task<ValidateLoginSVResponse> ValidateLogin(ValidateLoginSVRequest validateLoginSVRequest);
    Task<GenerateTokenJWTSVResponse> GenerateTokenJWT(GenerateTokenJWTSVRequest generateTokenJWTSVRequest);
}
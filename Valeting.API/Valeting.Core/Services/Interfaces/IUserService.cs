using Valeting.Core.Models.User;

namespace Valeting.Core.Services.Interfaces;

public interface IUserService
{
    Task<ValidateLoginSVResponse> ValidateLogin(ValidateLoginSVRequest validateLoginSVRequest);
    Task<GenerateTokenJWTSVResponse> GenerateTokenJWT(GenerateTokenJWTSVRequest generateTokenJWTSVRequest);
}
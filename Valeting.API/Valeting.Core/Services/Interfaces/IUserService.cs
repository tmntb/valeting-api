using Valeting.Core.Models.User;

namespace Valeting.Core.Services.Interfaces;

public interface IUserService
{
    Task<ValidateLoginSVResponse> ValidateLoginAsync(ValidateLoginSVRequest validateLoginSVRequest);
    Task<GenerateTokenJWTSVResponse> GenerateTokenJWTAsync(GenerateTokenJWTSVRequest generateTokenJWTSVRequest);
}
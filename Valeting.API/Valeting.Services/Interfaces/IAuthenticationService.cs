using Valeting.Business;

namespace Valeting.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthenticationDTO> GenerateTokenJWT(UserDTO userDTO);
    }
}


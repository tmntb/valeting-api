using System.Text;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;

using Valeting.Common.Messages;
using Valeting.Services.Interfaces;
using Valeting.Repositories.Interfaces;
using Valeting.Business.Authentication;
using System.Net;

namespace Valeting.Service;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<LoginDTO> ValidateLogin(UserDTO userDTO)
    {
        var loginDTO = new LoginDTO() { Errors = [] };
        if (userDTO == null)
        {
            loginDTO.Errors.Add(new()
            {
                Id = Guid.NewGuid(),
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Detail = Messages.UserDTONotPopulated
            });
            return loginDTO;
        }

        if(string.IsNullOrEmpty(userDTO.Username) || string.IsNullOrEmpty(userDTO.Password))
        {
            string errorMsg = string.IsNullOrEmpty(userDTO.Username) ? Messages.InvalidUsername : Messages.InvalidPassword;
            loginDTO.Errors.Add(new()
            {
                Id = Guid.NewGuid(),
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Detail = errorMsg
            });
            return loginDTO;
        }

        var userDTO_DB = await userRepository.FindUserByEmail(userDTO.Username);
        if (userDTO_DB == null || string.IsNullOrEmpty(userDTO_DB.Password))
        {
            loginDTO.Errors.Add(new()
            {
                Id = Guid.NewGuid(),
                ErrorCode = (int)HttpStatusCode.NotFound,
                Detail = Messages.UserNotFound
            });
            return loginDTO;
        }

        byte[] salt = Encoding.ASCII.GetBytes(userDTO_DB.Salt);

        /*
            * Criar Salt
        byte[] salt = new byte[128 / 8];
        using (RNGCryptoServiceProvider rngCsp = new())
        {
            rngCsp.GetNonZeroBytes(salt);
        }
        */

        var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(userDTO.Password, salt, KeyDerivationPrf.HMACSHA256, 100000, 256 / 8));
        loginDTO.Valid = userDTO_DB.Password.Equals(hashed);

        return loginDTO;
    }
}
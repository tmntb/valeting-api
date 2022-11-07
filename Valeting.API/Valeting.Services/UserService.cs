using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

using Valeting.Common.Messages;
using Valeting.Common.Exceptions;
using Valeting.Services.Interfaces;
using Valeting.Repositories.Interfaces;
using Valeting.Business.Authentication;

namespace Valeting.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new Exception(string.Format(Messages.NotInitializeRepository, "User Repository"));
        }

        public async Task<bool> ValidateLogin(UserDTO userDTO)
        {
            if (userDTO == null)
                throw new InputException(Messages.UserDTONotPopulated);

            if(string.IsNullOrEmpty(userDTO.Username) || string.IsNullOrEmpty(userDTO.Password))
            {
                string errorMsg = string.IsNullOrEmpty(userDTO.Username) ? Messages.InvalidUsername : Messages.InvalidPassword;
                throw new InputException(errorMsg);
            }

            var userDTO_DB = await _userRepository.FindUserByEmail(userDTO.Username);
            if (userDTO_DB == null || string.IsNullOrEmpty(userDTO_DB.Password))
                throw new NotFoundException(Messages.UserNotFound);

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

            return userDTO_DB.Password.Equals(hashed);
        }
    }
}

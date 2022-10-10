using System.Text;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;

using Valeting.Business;
using Valeting.Repositories.Interfaces;

namespace Valeting.Service
{
    public class UserService
    {
        private IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new Exception("userRepository is null");
        }

        public async Task<bool> ValidateLogin(UserDTO userDTO)
        {
            if (userDTO == null)
                throw new Exception("userDTO is null");

            if(string.IsNullOrEmpty(userDTO.Username) || string.IsNullOrEmpty(userDTO.Password))
            {
                string errorMsg = string.IsNullOrEmpty(userDTO.Username) ? "Username is null or empty" : "Password is null or empty";
                throw new Exception(errorMsg);
            }

            UserDTO userDTO_DB = await _userRepository.FindUserByEmail(userDTO.Username);
            if (userDTO_DB == null || string.IsNullOrEmpty(userDTO_DB.Password))
                return false;

            byte[] salt = Encoding.ASCII.GetBytes(userDTO_DB.Salt);

            /*
             * Criar Salt
            byte[] salt = new byte[128 / 8];
            using (RNGCryptoServiceProvider rngCsp = new())
            {
                rngCsp.GetNonZeroBytes(salt);
            }
            */

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(userDTO.Password, salt, KeyDerivationPrf.HMACSHA256, 100000, 256 / 8));

            return userDTO_DB.Password.Equals(hashed);
        }
    }
}

using Valeting.Business.Authentication;
using Valeting.Repositories.Entities;
using Valeting.Repositories.Interfaces;

namespace Valeting.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ValetingContext _valetingContext;

        public UserRepository(ValetingContext valetingContext)
        {
            this._valetingContext = valetingContext;
        }

        public async Task<UserDTO> FindUserByEmail(string username)
        {
            var applicationUser = await _valetingContext.ApplicationUsers.FindAsync(username);

            if (applicationUser == null)
                return null;

            return new UserDTO()
            {
                Id = applicationUser.Id,
                Username = username,
                Password = applicationUser.Password,
                Salt = applicationUser.Salt
            };
        }
    }
}

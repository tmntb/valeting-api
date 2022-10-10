using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Valeting.Business;
using Valeting.Repositories.Entities;
using Valeting.Repositories.Interfaces;

namespace Valeting.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DJValetingContext _valetingContext;

        public UserRepository(IConfiguration configuration)
        {
            this._valetingContext = new DJValetingContext(
                new DbContextOptionsBuilder<DJValetingContext>().UseSqlServer(configuration.GetConnectionString("DJValetingConnection")).Options);
        }

        public async Task<UserDTO> FindUserByEmail(string username)
        {
            ApplicationUser applicationUser = await _valetingContext.ApplicationUsers.FindAsync(username);

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

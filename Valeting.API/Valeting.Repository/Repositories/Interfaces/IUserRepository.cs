﻿using Valeting.Repository.Models.User;

namespace Valeting.Repository.Repositories.Interfaces;

public interface IUserRepository
{
    Task<UserDTO> FindUserByEmailAsync(string email);
}
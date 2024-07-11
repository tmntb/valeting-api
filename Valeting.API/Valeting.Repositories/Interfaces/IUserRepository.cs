﻿using Valeting.Repository.Models.User;

namespace Valeting.Repositories.Interfaces;

public interface IUserRepository
{
    Task<UserDTO> FindUserByEmail(string email);
}
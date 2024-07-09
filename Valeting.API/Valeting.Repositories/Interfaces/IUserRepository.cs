﻿using Valeting.Business.Authentication;

namespace Valeting.Repositories.Interfaces;

public interface IUserRepository
{
    Task<UserDTO> FindUserByEmail(string email);
}
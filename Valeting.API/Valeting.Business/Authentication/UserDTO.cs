﻿namespace Valeting.Business.Authentication;

public class UserDTO
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
}
﻿using System;
namespace Valeting.Business
{
    public class AuthenticationDTO
    {
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string TokenType { get; set; }
    }
}


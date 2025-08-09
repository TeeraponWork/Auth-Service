﻿using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    public class BcryptPasswordHasher : IPasswordHasher
    {
        public string Hash(string plain) => BCrypt.Net.BCrypt.HashPassword(plain);
        public bool Verify(string plain, string hash) => BCrypt.Net.BCrypt.Verify(plain, hash);
    }
}

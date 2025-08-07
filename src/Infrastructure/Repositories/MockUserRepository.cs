using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class MockUserRepository : IUserRepository
    {
        private readonly List<User> _users = new()
        {
            new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Password = "password" // ⚠️ plaintext password (สำหรับเท่านั้น!)
            },
            new User
            {
                Id = Guid.NewGuid(),
                Username = "test",
                Password = "123456"
            }
        };

        public User? GetUserByUsername(string username)
        {
            return _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }
    }
}

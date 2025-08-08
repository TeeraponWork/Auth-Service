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
                PasswordHash = "password"
            },
            new User
            {
                Id = Guid.NewGuid(),
                Username = "test",
                PasswordHash = "123456"
            }
        };
        public Task<User?> GetByUsernameAsync(string username)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            return Task.FromResult(user);
        }
    }
}

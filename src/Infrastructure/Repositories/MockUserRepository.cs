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
        private readonly List<User> _users = new();

        public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
            Task.FromResult(_users.SingleOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));

        public Task AddAsync(User user, CancellationToken ct = default)
        { _users.Add(user); return Task.CompletedTask; }

        public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
            Task.FromResult(_users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));
    }
}

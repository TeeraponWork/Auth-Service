using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private static readonly List<User> Users = new()
    {
        new User { Id = Guid.NewGuid(), Username = "admin", PasswordHash = "admin123" },
        new User { Id = Guid.NewGuid(), Username = "user", PasswordHash = "user123" }
    };

        public Task<User?> GetByUsernameAsync(string username)
        {
            var user = Users.FirstOrDefault(u => u.Username == username);
            return Task.FromResult(user);
        }
    }
}

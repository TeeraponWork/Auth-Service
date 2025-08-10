using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories
{
    public class UserRolesRepository : IUserRolesRepository
    {
        private readonly AuthDbContext _db;
        public UserRolesRepository(AuthDbContext db) => _db = db;
        public async Task AddAsync(UserRoles user, CancellationToken ct = default)
        {
            _db.UserRoles.Add(user);
            await _db.SaveChangesAsync(ct);
        }
    }
}

using Application.Auth;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

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
        public async Task<User?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _db.Users
                .AsNoTracking()
                .AsSplitQuery()
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId, ct);
        }
    }
}

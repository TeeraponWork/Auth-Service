using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _db;
        public UserRepository(AuthDbContext db) => _db = db;

        public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) {
            return _db.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Email == email, ct);
        }
        public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return _db.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == id, ct);
        }
        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);
        }

        public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
            _db.Users.AsNoTracking().AnyAsync(u => u.Email == email, ct);

        public async Task UpdateAsync(User user, CancellationToken ct = default)
        {
            var users = await _db.Users.Where(t => t.Id == user.Id).FirstOrDefaultAsync();
            user.CreatedBy = users.Id;
            user.UpdatedBy = users.Id;

            await _db.SaveChangesAsync(ct);
        }
    }
}

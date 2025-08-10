using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RolesRepository : IRolesRepository
    {
        private readonly AuthDbContext _db;
        public RolesRepository(AuthDbContext db) => _db = db;

        public Task<Roles?> GetByNameAsync(string name, CancellationToken ct = default)
        {
            return _db.Roles.AsNoTracking().SingleOrDefaultAsync(u => u.Name == name, ct);
        }
    }
}

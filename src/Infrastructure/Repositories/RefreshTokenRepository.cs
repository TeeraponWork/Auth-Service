using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AuthDbContext _db;
        public RefreshTokenRepository(AuthDbContext db) => _db = db;

        public async Task AddAsync(Guid userId, string tokenHash, DateTime expiresAt, DateTime createdAt, CancellationToken ct = default)
        {
            _db.RefreshTokens.Add(new RefreshToken
            {
                UserId = userId,
                TokenHash = tokenHash,
                ExpiresAt = expiresAt,
                CreatedAt = createdAt
            });
            await _db.SaveChangesAsync(ct);
        }

        public Task<IReadOnlyList<RefreshToken>> GetActiveByUserAsync(Guid userId, DateTime nowUtc, CancellationToken ct = default)
        {
            return _db.RefreshTokens.AsNoTracking()
                .Where(t => t.UserId == userId && t.RevokedAt == null && t.ExpiresAt > nowUtc)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(ct)
                .ContinueWith<IReadOnlyList<RefreshToken>>(t => t.Result, ct);
        }

        public async Task MarkRevokedAsync(Guid tokenId, DateTime whenUtc, CancellationToken ct = default)
        {
            var token = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Id == tokenId, ct);
            if (token is not null && token.RevokedAt is null)
            {
                token.RevokedAt = whenUtc;
                await _db.SaveChangesAsync(ct);
            }
        }

        public async Task<int> RevokeAllForUserAsync(Guid userId, DateTime whenUtc, CancellationToken ct = default)
        {
            var tokens = await _db.RefreshTokens
                .Where(t => t.UserId == userId && t.RevokedAt == null)
                .ToListAsync(ct);

            foreach (var t in tokens) t.RevokedAt = whenUtc;
            return await _db.SaveChangesAsync(ct);
        }
    }
}

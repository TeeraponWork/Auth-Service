namespace Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(Guid userId, string tokenHash, DateTime expiresAt, DateTime createdAt, CancellationToken ct = default);

        // ใช้หา refresh token ที่ยัง Active ของผู้ใช้ (ยังไม่หมดอายุ และยังไม่ถูก revoke)
        Task<IReadOnlyList<Domain.Entities.RefreshToken>> GetActiveByUserAsync(Guid userId, DateTime nowUtc, CancellationToken ct = default);

        // ใช้ mark ตัวเก่าเป็น revoke ตอนหมุน (rotation)
        Task MarkRevokedAsync(Guid tokenId, DateTime whenUtc, CancellationToken ct = default);

        // (ตัวเลือก) ใช้กรณีตรวจจับ reuse → เคลียร์ทิ้งทั้งหมด
        Task<int> RevokeAllForUserAsync(Guid userId, DateTime whenUtc, CancellationToken ct = default);
    }
}

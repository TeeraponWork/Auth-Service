using Application.Auth.Commands;
using Application.Common.Results;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using System.Security.Cryptography;

namespace Application.Auth.Handlers
{
    public sealed class RefreshHandler : IRequestHandler<RefreshCommand, Result<LoginResponse>>
    {
        private readonly IUserRepository _users;
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly IPasswordHasher _hasher;
        private readonly IJwtTokenGenerator _jwt;
        private readonly IUserRolesRepository _userRoles;

        public RefreshHandler(
            IUserRepository users,
            IRefreshTokenRepository refreshRepo,
            IPasswordHasher hasher,
            IJwtTokenGenerator jwt,
            IUserRolesRepository userRoles)
        {
            _users = users;
            _refreshRepo = refreshRepo;
            _hasher = hasher;
            _jwt = jwt;
            _userRoles = userRoles;
        }

        public async Task<Result<LoginResponse>> Handle(RefreshCommand req, CancellationToken ct)
        {
            var user = await _users.GetByIdAsync(req.UserId, ct);
            // ^ ถ้ายังไม่มีเมธอดนี้ ให้ GetByIdAsync แทน
            if (user is null || !user.IsActive) return Result<LoginResponse>.Failure("Unauthorized");

            // 1) หา active refresh tokens ของ user
            var actives = await _refreshRepo.GetActiveByUserAsync(user.Id, DateTime.UtcNow, ct);

            // 2) เทียบ hash กับตัวที่ส่งมา
            var matched = actives.FirstOrDefault(t => _hasher.Verify(req.RefreshToken, t.TokenHash));
            if (matched is null)
            {
                // (ตัวเลือก) ถ้าต้องการ reuse detection ขั้นสูง: _ = await _refreshRepo.RevokeAllForUserAsync(user.Id, DateTime.UtcNow, ct);
                return Result<LoginResponse>.Failure("Unauthorized");
            }

            // 3) ใช้สำเร็จ → revoke ตัวเดิม (rotation)
            await _refreshRepo.MarkRevokedAsync(matched.Id, DateTime.UtcNow, ct);

            var userRoles = await _userRoles.GetByUserIdAsync(user.Id, ct);
            var roleNames = userRoles.UserRoles
                .Select(ur => ur.Role.Name)
                .Distinct()
                .ToList();

            // 4) ออก access token ใหม่
            var access = _jwt.CreateToken(user, roles: roleNames, minutes: 30);

            // 5) ออก refresh token ใหม่ + เก็บ hash
            var refreshPlain = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();
            var refreshHash = _hasher.Hash(refreshPlain);
            await _refreshRepo.AddAsync(user.Id, refreshHash, DateTime.UtcNow.AddDays(14), DateTime.UtcNow, ct);

            return Result<LoginResponse>.Success(new LoginResponse
            {
                AccessToken = access,
                RefreshToken = refreshPlain
            });
        }
    }
}

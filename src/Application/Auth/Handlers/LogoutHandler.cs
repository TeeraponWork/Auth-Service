using Application.Auth.Commands;
using Application.Common.Results;
using Domain.Interfaces;
using MediatR;

namespace Application.Auth.Handlers
{
    public sealed class LogoutHandler : IRequestHandler<LogoutCommand, Result<Unit>>
    {
        private readonly IRefreshTokenRepository _refresh;
        private readonly IPasswordHasher _hasher;

        public LogoutHandler(IRefreshTokenRepository refresh, IPasswordHasher hasher)
        {
            _refresh = refresh;
            _hasher = hasher;
        }

        public async Task<Result<Unit>> Handle(LogoutCommand request, CancellationToken ct)
        {
            // ดึงเฉพาะ token ที่ยัง active ของผู้ใช้
            var actives = await _refresh.GetActiveByUserAsync(request.UserId, DateTime.UtcNow, ct);

            // หา token ที่ hash ตรงกับ refresh token ที่ client ส่งมา
            var matched = actives.FirstOrDefault(t => _hasher.Verify(request.RefreshToken, t.TokenHash));
            if (matched is null)
                return Result<Unit>.Failure("Refresh token not found.");

            // revoke ตัวที่ matched
            await _refresh.MarkRevokedAsync(matched.Id, DateTime.UtcNow, ct);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}

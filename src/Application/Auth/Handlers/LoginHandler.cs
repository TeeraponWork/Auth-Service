using Application.Auth.Commands;
using Application.Common.Results;
using Domain.Interfaces;
using MediatR;
using System;
using System.Security.Cryptography;

namespace Application.Auth.Handlers
{
    public sealed class LoginHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private readonly IUserRepository _users;
        private readonly IJwtTokenGenerator _jwt;
        private readonly IPasswordHasher _hasher;
        private readonly IRefreshTokenRepository _refreshTokens;

        public LoginHandler(
            IUserRepository users,
            IJwtTokenGenerator jwt,
            IPasswordHasher hasher,
            IRefreshTokenRepository refreshTokens
            )
        {
            _users = users;
            _jwt = jwt;
            _hasher = hasher;
            _refreshTokens = refreshTokens;
        }

        public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken ct)
        {
            var user = await _users.GetByEmailAsync(request.Email, ct);
            if (user is null || !user.IsActive)
                throw new UnauthorizedAccessException("Invalid credentials.");

            if (!_hasher.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials.");

            // 1) ออก access token
            var access = _jwt.CreateToken(user, roles: null, minutes: 30);

            // 2) สร้าง refresh token (plain) + hash
            var refreshPlain = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();
            var refreshHash = _hasher.Hash(refreshPlain);

            // 3) บันทึกลง repo (DB เก็บ HASH เท่านั้น)
            await _refreshTokens.AddAsync(
                userId: user.Id,
                tokenHash: refreshHash,
                expiresAt: DateTime.UtcNow.AddDays(14),
                createdAt: DateTime.UtcNow,
                ct: ct
            );

            // 4) ส่งคืน (คืน plain ให้ client)
            var result = new LoginResponse
            {
                AccessToken = access,
                RefreshToken = refreshPlain
            };

            return Result<LoginResponse>.Success(result);
        }
    }
}

using Application.Auth.Commands;
using Domain.Interfaces;
using MediatR;

namespace Application.Auth.Handlers
{
    public sealed class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IUserRepository _users;
        private readonly IJwtTokenGenerator _jwt;
        private readonly IPasswordHasher _hasher;

        public LoginHandler(IUserRepository users, IJwtTokenGenerator jwt, IPasswordHasher hasher)
        {
            _users = users;
            _jwt = jwt;
            _hasher = hasher;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken ct)
        {
            var user = await _users.GetByEmailAsync(request.Email, ct);
            if (user is null || !user.IsActive) throw new UnauthorizedAccessException("Invalid credentials.");

            if (!_hasher.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials.");

            var access = _jwt.CreateToken(user, roles: null, minutes: 30);
            var refreshPlain = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");

            return new LoginResponse { AccessToken = access, RefreshToken = refreshPlain };
        }
    }
}

using Application.Auth.Commands;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Auth.Handlers
{
    public sealed class RegisterHandler : IRequestHandler<RegisterCommand, RegisterResponse>
    {
        private readonly IUserRepository _users;
        private readonly IPasswordHasher _hasher;
        public RegisterHandler(IUserRepository users, IPasswordHasher hasher)
        {
            _users = users;
            _hasher = hasher;
        }

        public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Email/Password is required.");

            var exists = await _users.EmailExistsAsync(request.Email, cancellationToken);
            if (exists) throw new InvalidOperationException("Email already exists.");

            var user = new User
            {
                Email = request.Email,
                PasswordHash = _hasher.Hash(request.Password),
                DisplayName = request.DisplayName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _users.AddAsync(user, cancellationToken);

            return new RegisterResponse
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName
            };
        }
    }
}

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
        private readonly IRolesRepository _roles;
        private readonly IUserRolesRepository _userRoles;
        public RegisterHandler(
            IUserRepository users, 
            IPasswordHasher hasher, 
            IRolesRepository roles,
            IUserRolesRepository userRoles)
        {
            _users = users;
            _hasher = hasher;
            _roles = roles;
            _userRoles = userRoles;
        }

        public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                    throw new ArgumentException("Email/Password is required.");

                var exists = await _users.EmailExistsAsync(request.Email, cancellationToken);
                if (exists) throw new InvalidOperationException("Email already exists.");

                var now = DateTimeOffset.UtcNow;
                var user = new User
                {
                    Email = request.Email,
                    PasswordHash = _hasher.Hash(request.Password),
                    DisplayName = $"{request.FirstName ?? string.Empty} {request.LastName ?? string.Empty}".Trim(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Gender = request.Gender,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now,
                };
                await _users.AddAsync(user, cancellationToken);
                await _users.UpdateAsync(user, cancellationToken);

                var guidRoleUser = await _roles.GetByNameAsync("User", cancellationToken);
                var userRoles = new UserRoles
                {
                    UserId = user.Id,
                    RoleId = guidRoleUser.Id
                };
                await _userRoles.AddAsync(userRoles, cancellationToken);

                return new RegisterResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Gender = user.Gender
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

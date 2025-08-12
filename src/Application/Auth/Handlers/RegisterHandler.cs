using Application.Auth.Commands;
using Application.Common.Results;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Auth.Handlers
{
    public sealed class RegisterHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
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

        public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password) ||
                    string.IsNullOrWhiteSpace(request.ConfirmPassword))
                    throw new ArgumentException("Email/Password is required.");

                if(request.ConfirmPassword != request.Password)
                    throw new ValidationException("Password and confirmation do not match.");

                var exists = await _users.EmailExistsAsync(request.Email, cancellationToken);
                if (exists) throw new InvalidOperationException("Email already exists.");

                var now = DateTimeOffset.UtcNow;
                var user = new User
                {
                    Email = request.Email,
                    PasswordHash = _hasher.Hash(request.Password),
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

                var result = new RegisterResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                };

                return Result<RegisterResponse>.Success(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

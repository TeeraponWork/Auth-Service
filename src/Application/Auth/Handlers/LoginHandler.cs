using Application.Auth.Commands;
using Domain.Interfaces;
using MediatR;

namespace Application.Auth.Handlers
{
    public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public LoginHandler(IUserRepository userRepository, IJwtTokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);

            if (user == null || user.PasswordHash != request.Password)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var token = _tokenGenerator.GenerateToken(user.Id, user.Username);

            return new LoginResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Token = token
            };
        }
    }
}

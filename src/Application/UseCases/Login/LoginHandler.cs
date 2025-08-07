using Domain.Interfaces;

namespace Application.UseCases.Login
{
    public class LoginHandler
    {
        private readonly IUserRepository _userRepo;
        private readonly IJwtTokenGenerator _jwtGen;

        public LoginHandler(IUserRepository userRepo, IJwtTokenGenerator jwtGen)
        {
            _userRepo = userRepo;
            _jwtGen = jwtGen;
        }

        public string? Handle(LoginRequest request)
        {
            var user = _userRepo.GetUserByUsername(request.Username);
            if (user == null || user.Password != request.Password)
                return null;

            return _jwtGen.GenerateToken(user);
        }
    }
}

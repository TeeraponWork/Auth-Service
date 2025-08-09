using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string CreateToken(User user, IEnumerable<string>? roles = null, int minutes = 30);
    }
}

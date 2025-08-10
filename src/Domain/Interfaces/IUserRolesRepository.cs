using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserRolesRepository
    {
        Task AddAsync(UserRoles user, CancellationToken ct = default);
    }
}

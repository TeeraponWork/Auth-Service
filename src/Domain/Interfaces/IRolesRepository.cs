using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IRolesRepository
    {
        Task<Roles?> GetByNameAsync(string name, CancellationToken ct = default);
    }
}

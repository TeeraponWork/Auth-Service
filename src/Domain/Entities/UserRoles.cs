using System.Data;

namespace Domain.Entities
{
    public class UserRoles
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Roles Role { get; set; } = null!;
    }
}

namespace Domain.Entities
{
    public class Roles
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ICollection<UserRoles> UserRoles { get; set; } = new List<UserRoles>();
    }
}

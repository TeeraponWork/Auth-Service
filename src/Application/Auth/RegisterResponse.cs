using Domain.Enums;

namespace Application.Auth
{
    public sealed class RegisterResponse
    {
        public Guid Id { get; init; }
        public string Email { get; init; } = default!;
        public string? DisplayName { get; init; }
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public Gender? Gender { get; init; }
    }
}

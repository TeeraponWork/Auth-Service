namespace Application.Auth
{
    public sealed class RegisterResponse
    {
        public Guid Id { get; init; }
        public string Email { get; init; } = default!;
        public string? DisplayName { get; init; }
    }
}

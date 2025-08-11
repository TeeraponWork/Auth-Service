namespace Application.Auth
{
    public sealed class LoginResponse
    {
        public Guid User {  get; set; }
        public string AccessToken { get; init; } = default!;
        public string RefreshToken { get; init; } = default!;
    }
}

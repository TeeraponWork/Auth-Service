namespace Application.Auth
{
    public sealed class LoginResponse
    {
        public string AccessToken { get; init; } = default!;
        public string RefreshToken { get; init; } = default!;
    }
}

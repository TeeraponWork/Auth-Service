using MediatR;

namespace Application.Auth.Commands
{
    public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;
}

using Domain.Entities;
using MediatR;

namespace Application.Auth.Commands
{
    public sealed record RegisterCommand(
    string Email,
    string Password,
    string? DisplayName
    ) : IRequest<RegisterResponse>;
}

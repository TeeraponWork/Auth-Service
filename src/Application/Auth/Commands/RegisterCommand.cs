using Domain.Enums;
using MediatR;

namespace Application.Auth.Commands
{
    public sealed record RegisterCommand(
    string Email,
    string Password,
    string? FirstName,
    string? LastName,
    Gender Gender
    ) : IRequest<RegisterResponse>;
}

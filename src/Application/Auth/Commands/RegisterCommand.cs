using Application.Common.Results;
using MediatR;

namespace Application.Auth.Commands
{
    public sealed record RegisterCommand(
    string Email,
    string Password,
    string ConfirmPassword
    ) : IRequest<Result<RegisterResponse>>;
}

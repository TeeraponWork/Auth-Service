using Application.Common.Results;
using MediatR;

namespace Application.Auth.Commands
{
    public sealed record LogoutCommand(Guid UserId, string RefreshToken)
       : IRequest<Result<Unit>>;
}

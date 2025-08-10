using Application.Common.Results;
using MediatR;

namespace Application.Auth.Commands
{
    public sealed record RefreshCommand(Guid UserId, string RefreshToken)
        : IRequest<Result<LoginResponse>>;
}

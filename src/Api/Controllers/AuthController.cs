using Api.BaseController;
using Application.Auth;
using Application.Auth.Commands;
using Application.Common.Results;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AuthController : ApiBaseController
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand cmd)
        {
            try
            {
                var result = await _mediator.Send(cmd);
                return HandleResponse(result);
            }
            catch (Exception ex) 
            {
                return HandleErrorResponse(ex.Message);
            }
        }

        [HttpPost("refresh")]
        public Task<Result<LoginResponse>> Refresh([FromBody] RefreshCommand body, CancellationToken ct)
        => _mediator.Send(new RefreshCommand(body.UserId, body.RefreshToken), ct);
    }
}

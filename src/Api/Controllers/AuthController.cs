using Api.BaseController;
using Application.Auth.Commands;
using Asp.Versioning;
using MediatR;
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

        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            return Ok("ok");
        }

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
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand cmd)
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
        public async Task<IActionResult> Refresh([FromBody] RefreshCommand body, CancellationToken ct)
        {
            try
            {
                var result = await _mediator.Send(new RefreshCommand(body.UserId, body.RefreshToken), ct);
                return HandleResponse(result);
            }
            catch (Exception ex)
            {
                return HandleErrorResponse(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand body, CancellationToken ct)
        {
            try
            {
                var result = await _mediator.Send(new RegisterCommand
                    (body.Email,body.Password,body.ConfirmPassword), ct);
                return HandleResponse(result);
            }
            catch (Exception ex)
            {
                return HandleErrorResponse(ex.Message);
            }
        }
    }
}

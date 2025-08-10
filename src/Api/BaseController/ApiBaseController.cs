using Microsoft.AspNetCore.Mvc;

namespace Api.BaseController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiBaseController : ControllerBase
    {
        protected IActionResult HandleResponse<T>(T value)
        {
            if (value == null)
            {
                return NotFound();
            }

            return Ok(value);
        }
        protected IActionResult HandleErrorResponse(string errorMessage)
        {
            return BadRequest(new { message = errorMessage });
        }
    }
}

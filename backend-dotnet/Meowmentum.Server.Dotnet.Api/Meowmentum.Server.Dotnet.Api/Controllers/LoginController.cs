using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Requests;
using Meowmentum.Server.Dotnet.Shared.Results;
using Meowmentum.Server.Dotnet.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Data;

namespace Meowmentum.Server.Dotnet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] Shared.Requests.LoginRequest loginRequest, CancellationToken token = default)
        {
            var loginResult = await _loginService.LoginAsync(loginRequest, token);
            if (!loginResult.IsSuccess)
            {
                return BadRequest(new { Message = loginResult.ErrorMessage });
            }

            return Ok(new LoginResponse { Token = loginResult.Data });
        }
    }
}

using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Requests;
using Meowmentum.Server.Dotnet.Shared.Requests.Registration;
using Meowmentum.Server.Dotnet.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Meowmentum.Server.Dotnet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken token = default)
    {
        var result = await authService.RegisterUserAsync(request, token);

        if (result.IsSuccess)
            return Ok(result.Message);

        return BadRequest(result.ErrorMessage);
    }

    [HttpPost("verify-otp")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifyOtp([FromBody] OtpValidationRequest request, CancellationToken token = default)
    {
        var result = await authService.VerifyOtpAsync(request, token);

        if (result.IsSuccess)
            return Ok(result.Message);

        return BadRequest(result.ErrorMessage);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest, CancellationToken token = default)
    {
        var loginResult = await authService.LoginAsync(loginRequest, token);
        if (!loginResult.IsSuccess)
        {
            return BadRequest(loginResult.ErrorMessage);
        }

        return Ok(new LoginResponse { Token = loginResult.Data });
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Logout(CancellationToken token = default)
    {
        var jwtToken = HttpContext.Items["JwtToken"] as string;

        var result = await authService.LogoutAsync(jwtToken, token);

        if (result.IsSuccess)
            return Ok(result.Message);

        return BadRequest(result.ErrorMessage);
    }
}

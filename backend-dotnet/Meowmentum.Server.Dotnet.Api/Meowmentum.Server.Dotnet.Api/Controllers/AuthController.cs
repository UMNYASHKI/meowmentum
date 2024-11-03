using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Requests.Registration;
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
            return Ok("User registered successfully. Please verify your email!");

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
            return Ok("Email verified successfully!");

        return BadRequest(result.ErrorMessage);
    }
}

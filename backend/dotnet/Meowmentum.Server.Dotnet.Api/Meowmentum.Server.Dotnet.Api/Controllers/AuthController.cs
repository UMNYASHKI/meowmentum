using Meowmentum.Server.Dotnet.Api.Helpers;
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
[ValidateModel]
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

    [HttpPost("send-otp")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendOtp([FromBody] OtpSendRequest request, CancellationToken token = default)
    {
        var result = await authService.SendOtpAsync(request, token);

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

        return Ok(new LoginResponse(loginResult.Data));
    }

    [HttpPost("send-reset-otp")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendOtp([FromBody] PasswordResetRequest request, CancellationToken ct = default)
    {
        var result = await authService.SendResetOtpAsync(request.Email, ct);

        if (result.IsSuccess)
            return Ok(result.Message);

        return BadRequest(result.ErrorMessage);
    }

    [HttpPost("verify-reset-otp")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> VerifyResetOtp([FromBody] OtpValidationRequest request, CancellationToken ct = default)
    {
        var result = await authService.VerifyOtpAsync(request, ct);

        if (result.IsSuccess)
        {
            return Ok(new ResetPasswordResponse(result.Message));
        }

        return BadRequest(result.ErrorMessage);
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ResetPassword([FromBody] PasswordUpdateRequest request, CancellationToken ct = default)
    {
        var result = await authService.UpdatePasswordAsync(request, ct);

        if (result.IsSuccess)
            return Ok(result.Message);

        return BadRequest(result.ErrorMessage);
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Logout(CancellationToken token = default)
    {
        var result = await authService.LogoutAsync(token);

        if (result.IsSuccess)
            return Ok(result.Message);

        return BadRequest(result.ErrorMessage);
    }
}

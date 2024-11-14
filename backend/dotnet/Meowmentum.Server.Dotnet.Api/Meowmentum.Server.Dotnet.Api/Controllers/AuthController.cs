﻿using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Requests.Registration;
using Meowmentum.Server.Dotnet.Shared.Responses;
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
    public async Task<IActionResult> Login([FromBody] Shared.Requests.LoginRequest loginRequest, CancellationToken token = default)
    {
        var loginResult = await authService.LoginAsync(loginRequest, token);
        if (!loginResult.IsSuccess)
        {
            return BadRequest(new { Message = loginResult.ErrorMessage });
        }

        return Ok(new LoginResponse { Token = loginResult.Data });
    }
}

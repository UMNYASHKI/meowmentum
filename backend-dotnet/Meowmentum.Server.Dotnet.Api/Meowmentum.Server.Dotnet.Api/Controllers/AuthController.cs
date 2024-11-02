using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Requests.Registration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Meowmentum.Server.Dotnet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        if (await _authService.RegisterUserAsync(request))
            return Ok("User registered successfully. Please verify your email!");

        return BadRequest("Registration failed!");
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(OtpValidationRequest request)
    {
        if (await _authService.VerifyOtpAsync(request))
            return Ok("Email verified successfully!");

        return BadRequest("OTP verification failed!");
    }
}

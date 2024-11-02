using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Shared.Requests.Registration;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Infrastructure.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IOtpService _otpService;

    public AuthService(UserManager<AppUser> userManager, IEmailService emailService, IOtpService otpService)
    {
        _userManager = userManager;
        _emailService = emailService;
        _otpService = otpService;
    }

    public async Task<bool> RegisterUserAsync(RegisterUserRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return false;
        }

        var user = new AppUser { UserName = request.Email, Email = request.Email };
        var response = await _userManager.CreateAsync(user, request.Password);

        if (response.Succeeded)
        {
            var otp = _otpService.GenerateOtp();
            await _emailService.SendOtpByEmailAsync(user.Email, otp);
            await _otpService.SaveOtpForUserAsync(user.Id, otp);
            return true;
        }

        return false;
    }

    public async Task<bool> VerifyOtpAsync(OtpValidationRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user != null && await _otpService.ValidateOtpAsync(user.Id, request.OtpCode))
        {
            //user.IsEmailVerified = true;
            await _userManager.UpdateAsync(user);
            return true;
        }

        return false;
    }
}

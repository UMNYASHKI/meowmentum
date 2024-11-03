using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Shared.Requests.Registration;
using Meowmentum.Server.Dotnet.Shared.Results;
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

    public async Task<Result<bool>> RegisterUserAsync(RegisterUserRequest request)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return Result.Failure<bool>(ResultMessages.User.EmailAlreadyExists);
            }

            var user = new AppUser { UserName = request.UserName, Email = request.Email };
            var response = await _userManager.CreateAsync(user, request.Password);

            if (response.Succeeded)
            {
                var otp = _otpService.GenerateOtp();
                await _emailService.SendOtpByEmailAsync(user.Email, otp);
                await _otpService.SaveOtpForUserAsync(user.Id, otp);
                return Result.Success(true);
            }

            return Result.Failure<bool>(ResultMessages.Registration.FailedToCreateUser + string.Join('\n', response.Errors));
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<bool>(ResultMessages.Registration.OperationError);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<bool>(ResultMessages.Registration.InvalidArgument);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"{ResultMessages.Registration.UnexpectedError} {ex.Message}");
        }
    }

    public async Task<Result<bool>> VerifyOtpAsync(OtpValidationRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure<bool>(ResultMessages.User.UserNotFound);
            }

            var otpValidation = await _otpService.ValidateOtpAsync(user.Id, request.OtpCode);
            if (otpValidation.IsSuccess)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
                return Result.Success(true);
            }

            return Result.Failure<bool>(ResultMessages.User.InvalidOtpCode);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<bool>(ResultMessages.Otp.OperationError);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<bool>(ResultMessages.Otp.InvalidArgument);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"{ResultMessages.Otp.UnexpectedError} {ex.Message}");
        }
    }
}

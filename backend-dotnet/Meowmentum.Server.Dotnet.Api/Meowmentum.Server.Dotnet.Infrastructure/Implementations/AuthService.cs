﻿using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Infrastructure.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Requests.Registration;
using Meowmentum.Server.Dotnet.Shared.Requests;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.AspNetCore.Identity;
using Meowmentum.Server.Dotnet.Infrastructure.HelperServices;

namespace Meowmentum.Server.Dotnet.Infrastructure.Implementations;

public class AuthService(
    UserManager<AppUser> userManager, 
    IEmailService emailService, 
    IOtpManager otpService, 
    ITokenService tokenService,
    ITokenBlackListManager tokenBlackListManager) : IAuthService
{
    public async Task<Result<bool>> RegisterUserAsync(RegisterUserRequest request, CancellationToken ct = default)
    {
        try
        {
            var existingUser = await userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return Result.Failure<bool>(ResultMessages.User.EmailAlreadyExists);
            }

            var user = new AppUser { UserName = request.UserName, Email = request.Email };
            var response = await userManager.CreateAsync(user, request.Password);

            if (response.Succeeded)
            {
                var otp = otpService.GenerateOtp();
                var saveOtpResult = await otpService.SaveOtpForUserAsync(user.Id, otp, ct);
                if (!saveOtpResult.IsSuccess)
                {
                    return Result.Failure<bool>(ResultMessages.Otp.FailedToSaveOtp);
                }

                await emailService.SendOtpByEmailAsync(user.Email, otp, ct);
                
                return Result.Success(true, ResultMessages.Registration.Success);
            }

            return Result.Failure<bool>(ResultMessages.Registration.FailedToCreateUser + string.Join('\n', response.Errors));
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"{ResultMessages.Registration.UnexpectedError} {ex.Message}");
        }
    }

    public async Task<Result<bool>> VerifyOtpAsync(OtpValidationRequest request, CancellationToken ct = default)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure<bool>(ResultMessages.User.UserNotFound);
            }

            var otpValidation = await otpService.ValidateOtpAsync(user.Id, request.OtpCode, ct);
            if (otpValidation.IsSuccess)
            {
                user.EmailConfirmed = true;
                await userManager.UpdateAsync(user);
                return Result.Success(true, ResultMessages.Otp.OtpVerified);
            }

            return Result.Failure<bool>(ResultMessages.User.InvalidOtpCode);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"{ResultMessages.Otp.UnexpectedError} {ex.Message}");
        }
    }

    public async Task<Result<string>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure<string>(ResultMessages.User.UserNotFound);
            }

            if (!await userManager.CheckPasswordAsync(user, request.Password))
            {
                return Result.Failure<string>(ResultMessages.User.WrongPassword);
            }

            var tokenString = tokenService.GetToken(user);
            return Result.Success(tokenString);
        }
        catch (Exception ex)
        {
            return Result.Failure<string>($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<Result<bool>> LogoutAsync(string token, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(token))
        {
            return Result.Failure<bool>(ResultMessages.User.InvalidToken);
        }

        var blacklistResult = await tokenBlackListManager.AddTokenToBlackList(token, ct);
        if (!blacklistResult.IsSuccess)
        {
            return Result.Failure<bool>(blacklistResult.ErrorMessage);
        }

        return Result.Success(true, ResultMessages.User.LogoutSuccess);
    }
}
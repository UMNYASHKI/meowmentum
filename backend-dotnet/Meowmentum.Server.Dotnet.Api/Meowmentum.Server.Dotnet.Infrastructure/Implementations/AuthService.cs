using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Shared.Requests.Registration;
using Meowmentum.Server.Dotnet.Shared.Requests;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Infrastructure.Implementations;

public class AuthService(UserManager<AppUser> userManager, IEmailService emailService, IOtpManager otpService, ITokenService jwtService) 
    : IAuthService
{
    public async Task<Result<bool>> RegisterUserAsync(RegisterUserRequest request, CancellationToken token = default)
    {
        try
        {
            token.ThrowIfCancellationRequested();

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
                await emailService.SendOtpByEmailAsync(user.Email, otp, token);
                await otpService.SaveOtpForUserAsync(user.Id, otp, token);
                return Result.Success(true, ResultMessages.Registration.Success);
            }

            return Result.Failure<bool>(ResultMessages.Registration.FailedToCreateUser + string.Join('\n', response.Errors));
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<bool>(ResultMessages.Cancellation.OperationCanceled);
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

    public async Task<Result<bool>> VerifyOtpAsync(OtpValidationRequest request, CancellationToken token = default)
    {
        try
        {
            token.ThrowIfCancellationRequested();

            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure<bool>(ResultMessages.User.UserNotFound);
            }

            var otpValidation = await otpService.ValidateOtpAsync(user.Id, request.OtpCode, token);
            if (otpValidation.IsSuccess)
            {
                user.EmailConfirmed = true;
                await userManager.UpdateAsync(user);
                return Result.Success(true, ResultMessages.Otp.OtpVerified);
            }

            return Result.Failure<bool>(ResultMessages.User.InvalidOtpCode);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<bool>(ResultMessages.Cancellation.OperationCanceled);
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

    public async Task<Result<string>> LoginAsync(LoginRequest request, CancellationToken token = default)
    {
        try
        {
            token.ThrowIfCancellationRequested();

            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure<string>(ResultMessages.User.UserNotFound);
            }

            if (!await userManager.CheckPasswordAsync(user, request.Password))
            {
                return Result.Failure<string>(ResultMessages.User.WrongPassword);
            }

            var tokenString = jwtService.GetToken(user);
            return Result.Success(tokenString);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<string>(ResultMessages.Cancellation.OperationCanceled);
        }
        catch (Exception ex)
        {
            return Result.Failure<string>($"Unexpected error: {ex.Message}");
        }
    }

}

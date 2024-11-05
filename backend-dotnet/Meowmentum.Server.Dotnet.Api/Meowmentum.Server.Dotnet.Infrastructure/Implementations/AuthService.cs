using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Infrastructure.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Requests.Registration;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.AspNetCore.Identity;

namespace Meowmentum.Server.Dotnet.Infrastructure.Implementations;

public class AuthService(UserManager<AppUser> userManager, IEmailService emailService, IOtpManager otpService) 
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
                var saveOtpResult = await otpService.SaveOtpForUserAsync(user.Id, otp, token);
                if (!saveOtpResult.IsSuccess)
                {
                    return Result.Failure<bool>(ResultMessages.Otp.FailedToSaveOtp);
                }

                await emailService.SendOtpByEmailAsync(user.Email, otp, token);
                
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
}

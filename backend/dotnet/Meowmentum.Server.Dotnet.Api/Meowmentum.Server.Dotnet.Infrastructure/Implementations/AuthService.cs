using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Infrastructure.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Requests.Registration;
using Meowmentum.Server.Dotnet.Shared.Requests;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.AspNetCore.Identity;
using Meowmentum.Server.Dotnet.Infrastructure.HelperServices;

namespace Meowmentum.Server.Dotnet.Infrastructure.Implementations;

public class AuthService(UserManager<AppUser> userManager, IEmailService emailService, IOtpManager otpService, ITokenService tokenService) 
    : IAuthService
{
    public async Task<Result<bool>> RegisterUserAsync(RegisterUserRequest request, CancellationToken token = default)
    {
        try
        {
            var existingUser = await userManager.FindByEmailAsync(request.Email);
            if (existingUser is not null)
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
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
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
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
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
        catch (OperationCanceledException)
        {
            return Result.Failure<string>(ResultMessages.Cancellation.OperationCanceled);
        }
        catch (Exception ex)
        {
            return Result.Failure<string>($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<Result<bool>> SendResetOtpAsync(string email, CancellationToken ct = default)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return Result.Failure<bool>(ResultMessages.User.UserNotFound);
            }

            var otp = otpService.GenerateOtp();
            var saveOtpResult = await otpService.SaveOtpForUserAsync(user.Id, otp, ct);
            if (!saveOtpResult.IsSuccess)
            {
                return Result.Failure<bool>(ResultMessages.Otp.FailedToSaveOtp);
            }

            await emailService.SendOtpByEmailAsync(user.Email, otp, ct);
            return Result.Success(true);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<bool>(ResultMessages.Cancellation.OperationCanceled);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"{ResultMessages.Registration.UnexpectedError} {ex.Message}");
        }
    }

    public async Task<Result<string>> VerifyResetOtpAsync(OtpValidationRequest request, CancellationToken ct = default)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return Result.Failure<string>(ResultMessages.User.UserNotFound);
            }

            var otpValidation = await otpService.ValidateOtpAsync(user.Id, request.OtpCode, ct);
            if (!otpValidation.IsSuccess)
            {
                return Result.Failure<string>(ResultMessages.User.InvalidOtpCode);
            }

            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

            return Result.Success(resetToken, ResultMessages.Otp.OtpVerified);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<string>(ResultMessages.Cancellation.OperationCanceled);
        }
        catch (Exception ex)
        {
            return Result.Failure<string>($"{ResultMessages.Registration.UnexpectedError} {ex.Message}");
        }
    }

    public async Task<Result<bool>> UpdatePasswordAsync(PasswordUpdateRequest request, CancellationToken ct = default)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return Result.Failure<bool>(ResultMessages.User.UserNotFound);
            }

            var resetTokenResult = await userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "ResetPassword", request.ResetToken);
            if (!resetTokenResult)
            {
                return Result.Failure<bool>(ResultMessages.User.InvalidResetToken);
            }

            var passwordUpdateResult = await userManager.ResetPasswordAsync(user, request.ResetToken, request.NewPassword);
            if (!passwordUpdateResult.Succeeded)
            {
                return Result.Failure<bool>(string.Join('\n', passwordUpdateResult.Errors.Select(e => e.Description)));
            }

            return Result.Success(true, ResultMessages.User.PasswordUpdated);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<bool>(ResultMessages.Cancellation.OperationCanceled);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"{ResultMessages.Registration.UnexpectedError} {ex.Message}");
        }
    }

}

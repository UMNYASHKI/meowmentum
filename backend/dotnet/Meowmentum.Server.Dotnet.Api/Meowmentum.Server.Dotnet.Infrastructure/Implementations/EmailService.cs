using AutoMapper;
using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Proto.Email;
using Meowmentum.Server.Dotnet.Shared.Requests.Email;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.Extensions.Logging;
using EmailClient = Meowmentum.Server.Dotnet.Proto.Email.EmailService.EmailServiceClient;

namespace Meowmentum.Server.Dotnet.Infrastructure.Implementations;

public class EmailService(EmailClient emailClient, ILogger<IEmailService> logger, IMapper mapper) : IEmailService
{
    public async Task<Result<bool>> SendOtpByEmailAsync(OtpEmailSendingRequest sendingRequest, CancellationToken ct = default) //todo create retry logic
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            var request = mapper.Map<RegistrationConfirmationRequest>(sendingRequest);

            logger.LogInformation("Sending OTP to email: {To}", sendingRequest.Email);
            var response = await emailClient.RegistrationConfirmationAsync(request, cancellationToken: ct);
            if (response is null)
            {
                logger.LogError("Failed to send email {To}", sendingRequest.Email);
                return Result.Failure<bool>(ResultMessages.Email.FailToSend);
            }

            logger.LogInformation("OTP sent to email: {To}", sendingRequest.Email);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"{ResultMessages.Email.UnexpectedError} {ex.Message}");
        }
    }

    public async Task<Result<bool>> SendResetPasswordEmailAsync(ResetPasswordEmailSendingRequest sendingRequest, CancellationToken ct = default)
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            var request = mapper.Map<PasswordResetRequest>(sendingRequest);

            logger.LogInformation("Sending reset password email to email: {To}", sendingRequest.Email);
            var response = await emailClient.PasswordResetAsync(request, cancellationToken: ct);
            if (response is null)
            {
                logger.LogError("Failed to send email {To}", sendingRequest.Email);
                return Result.Failure<bool>(ResultMessages.Email.FailToSend);
            }

            logger.LogInformation("Reset password email sent to email: {To}", sendingRequest.Email);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"{ResultMessages.Email.UnexpectedError} {ex.Message}");
        }
    }
}

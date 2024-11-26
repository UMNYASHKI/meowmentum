using Meowmentum.Server.Dotnet.Shared.Requests.Email;
using Meowmentum.Server.Dotnet.Shared.Results;


namespace Meowmentum.Server.Dotnet.Business.Abstractions;

public interface IEmailService
{
    Task<Result<bool>> SendOtpByEmailAsync(OtpEmailSendingRequest sendingRequest, CancellationToken ct = default);
    Task<Result<bool>> SendResetPasswordEmailAsync(ResetPasswordEmailSendingRequest sendingRequest, CancellationToken ct = default);
}

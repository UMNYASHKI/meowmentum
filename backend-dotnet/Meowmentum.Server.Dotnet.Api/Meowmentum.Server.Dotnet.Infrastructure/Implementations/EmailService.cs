using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Infrastructure.Implementations;

public class EmailService : IEmailService
{
    public async Task<Result<bool>> SendOtpByEmailAsync(string email, string otp)
    {
        try
        {
            await Task.Delay(100);
            Console.WriteLine($"OTP {otp} sent to {email}");

            return Result.Success(true);
        }
        catch (HttpRequestException ex)
        {
            return Result.Failure<bool>($"{ResultMessages.Email.NetworkError} {ex.Message}");
        }
        catch (TimeoutException ex)
        {
            return Result.Failure<bool>($"{ResultMessages.Email.TimeoutError} {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"{ResultMessages.Email.UnexpectedError} {ex.Message}");
        }
    }
}

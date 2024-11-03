using Meowmentum.Server.Dotnet.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Business.Abstractions;

public interface IEmailService
{
    Task<Result<bool>> SendOtpByEmailAsync(string email, string otp, CancellationToken token);
}

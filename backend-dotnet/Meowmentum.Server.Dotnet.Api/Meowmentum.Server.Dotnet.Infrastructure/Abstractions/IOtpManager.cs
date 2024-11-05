using Meowmentum.Server.Dotnet.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Infrastructure.Abstractions;

public interface IOtpManager
{
    string GenerateOtp();
    Task<Result<bool>> SaveOtpForUserAsync(long userId, string otp, CancellationToken token = default);
    Task<Result<bool>> ValidateOtpAsync(long userId, string otp, CancellationToken token = default);
}

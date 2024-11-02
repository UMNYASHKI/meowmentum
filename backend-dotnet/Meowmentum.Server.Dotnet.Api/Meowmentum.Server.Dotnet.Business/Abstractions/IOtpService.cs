using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Business.Abstractions;

public interface IOtpService
{
    string GenerateOtp();
    Task SaveOtpForUserAsync(long userId, string otp);
    Task<bool> ValidateOtpAsync(long userId, string otp);
}

using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Shared.Requests.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Business.Abstractions;

public interface IAuthService
{
    Task<bool> RegisterUserAsync(RegisterUserRequest request);
    Task<bool> VerifyOtpAsync(OtpValidationRequest request);
}

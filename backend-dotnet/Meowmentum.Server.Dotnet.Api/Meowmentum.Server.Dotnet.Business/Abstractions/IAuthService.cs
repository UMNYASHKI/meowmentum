using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Shared.Requests;
using Meowmentum.Server.Dotnet.Shared.Requests.Registration;
using Meowmentum.Server.Dotnet.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Business.Abstractions;

public interface IAuthService
{
    Task<Result<bool>> RegisterUserAsync(RegisterUserRequest request, CancellationToken token = default);
    Task<Result<bool>> VerifyOtpAsync(OtpValidationRequest request, CancellationToken token = default);
    Task<Result<string>> LoginAsync(LoginRequest request, CancellationToken token = default);
}

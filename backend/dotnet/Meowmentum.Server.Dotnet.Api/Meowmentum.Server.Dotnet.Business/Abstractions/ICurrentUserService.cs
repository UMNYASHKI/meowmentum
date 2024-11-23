using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Business.Abstractions;

public interface ICurrentUserService
{
    Task<Result<AppUser>> GetCurrentUser(CancellationToken ct = default);
    Task<Result<long>> GetCurrentUserId(CancellationToken ct = default);
}

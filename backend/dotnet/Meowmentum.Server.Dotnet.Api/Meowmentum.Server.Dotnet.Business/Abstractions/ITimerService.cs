using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Shared.Requests;
using Meowmentum.Server.Dotnet.Shared.Requests.Task;
using Meowmentum.Server.Dotnet.Shared.Responses;
using Meowmentum.Server.Dotnet.Shared.Responses.Task;
using Meowmentum.Server.Dotnet.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Business.Abstractions
{
    public interface ITimeService
    {
        Task<Result<bool>> StartTimerAsync(long userId, long taskId, CancellationToken ct = default);
        Task<Result<bool>> StopTimerAsync(long userId, long taskId, CancellationToken ct = default);
        Task<Result<bool>> UpdateTimerAsync(long timerId, long userId, TimerUpdateRequest updateRequest, CancellationToken ct = default);
        Task<Result<bool>> DeleteTimerAsync(long userId, long timerId, CancellationToken ct = default);
        Task<Result<IEnumerable<TimeInterval>>> GetTimersAsync(long userId, long? taskId, long? timeIntervalId, CancellationToken ct = default);
    }
}

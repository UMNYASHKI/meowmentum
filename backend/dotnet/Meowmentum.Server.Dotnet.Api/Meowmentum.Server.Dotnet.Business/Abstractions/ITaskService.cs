using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Shared.Requests.Task;
using Meowmentum.Server.Dotnet.Shared.Responses.Task;
using Meowmentum.Server.Dotnet.Shared.Results;
using Task = Meowmentum.Server.Dotnet.Core.Entities.Task;

namespace Meowmentum.Server.Dotnet.Business.Abstractions;

public interface ITaskService
{
    Task<Result<bool>> CreateTaskAsync(long userId, Task task, CancellationToken ct = default);
    Task<Result<bool>> UpdateTaskAsync(long userId, Task task, CancellationToken ct = default);
    Task<Result<bool>> UpsertTaskAsync(long userId, Task task, long? taskId, CancellationToken ct = default);
    Task<Result<bool>> DeleteTaskAsync(long userId, long taskId, CancellationToken ct = default);
    Task<Result<IEnumerable<TaskResponse>>> GetTasksAsync(long userId, TaskFilterRequest filterRequest, CancellationToken ct = default);
}

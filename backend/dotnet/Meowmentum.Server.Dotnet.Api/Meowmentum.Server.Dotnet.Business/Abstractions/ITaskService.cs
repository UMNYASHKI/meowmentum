using Meowmentum.Server.Dotnet.Shared.Requests.Task;
using Meowmentum.Server.Dotnet.Shared.Responses.Task;
using Meowmentum.Server.Dotnet.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Business.Abstractions
{
    public interface ITaskService
    {
        Task<Result<TaskResponse>> CreateTaskAsync(CreateTaskRequest taskCreateRequest, CancellationToken ct = default);
        Task<Result<TaskResponse>> GetTaskByIdAsync(long taskId, CancellationToken ct = default);
        Task<Result<IEnumerable<TaskResponse>>> GetAllUserTasks(CancellationToken ct = default);
        Task<Result<TaskResponse>> UpdateTaskAsync(long taskId, CreateTaskRequest updateRequest, CancellationToken ct = default);
        Task<Result<bool>> DeleteTaskAsync(long taskId, CancellationToken ct = default);
        Task<Result<IEnumerable<TaskResponse>>> GetTasksByFilterAsync(TaskFilterRequest filterRequest, CancellationToken ct = default);
    }
}

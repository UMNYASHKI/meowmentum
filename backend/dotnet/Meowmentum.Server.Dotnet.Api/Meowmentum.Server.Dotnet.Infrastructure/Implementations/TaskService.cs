using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Results;
using Task = Meowmentum.Server.Dotnet.Core.Entities.Task;
using Meowmentum.Server.Dotnet.Persistence;
using Meowmentum.Server.Dotnet.Shared.Requests.Task;
using Microsoft.EntityFrameworkCore;
using Meowmentum.Server.Dotnet.Shared.Responses.Task;
using AutoMapper;
using System.Linq;


namespace Meowmentum.Server.Dotnet.Infrastructure.Implementations
{
    public class TaskService(
        ApplicationDbContext context,
        IMapper mapper,
        IAuthService authService) : ITaskService
    {

        public async Task<Result<TaskResponse>> CreateTaskAsync(CreateTaskRequest taskCreateRequest, CancellationToken ct = default)
        {
            var userId = authService.GetCurrentUserId();

            var task = mapper.Map<Task>(taskCreateRequest);
            task.UserId = userId;

            await context.Tasks.AddAsync(task, ct);
            await context.SaveChangesAsync(ct);

            var taskWithTag = await context.Tasks
                .FirstOrDefaultAsync(t => t.Id == task.Id, cancellationToken: ct);

            var taskResponse = mapper.Map<TaskResponse>(task);
            return Result.Success(taskResponse);
        }

        public async Task<Result<TaskResponse>> UpdateTaskAsync(long taskId, CreateTaskRequest updateRequest, CancellationToken ct = default)
        {
            var userId = authService.GetCurrentUserId();

            var task = await context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken: ct);

            if (task == null)
                return Result.Failure<TaskResponse>(ResultMessages.Task.TaskNotFound);

            if (task.UserId != userId)
                return Result.Failure<TaskResponse>(ResultMessages.Task.UnauthorizedUpdate);

            mapper.Map(updateRequest, task);
            await context.SaveChangesAsync(ct);

            return Result.Success(mapper.Map<TaskResponse>(task));
        }

        public async Task<Result<bool>> DeleteTaskAsync(long taskId, CancellationToken ct = default)
        {
            var userId = authService.GetCurrentUserId();

            var task = await context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken: ct);

            if (task == null)
                return Result.Failure<bool>(ResultMessages.Task.TaskNotFound);

            if (task.UserId != userId)
                return Result.Failure<bool>(ResultMessages.Task.UnauthorizedDelete);

            context.Tasks.Remove(task);
            await context.SaveChangesAsync(ct);

            return Result.Success(true);
        }

        public async Task<Result<IEnumerable<TaskResponse>>> GetTasksAsync(TaskFilterRequest filterRequest, CancellationToken ct = default)
        {
            var userId = authService.GetCurrentUserId();

            if (filterRequest.TaskId.HasValue)
            {
                var task = await TaskByIdAsync(filterRequest.TaskId.Value, ct);

                if (task == null)
                    return Result.Failure<IEnumerable<TaskResponse>>(ResultMessages.Task.TaskNotFound);

                if (task.UserId != userId)
                    return Result.Failure<IEnumerable<TaskResponse>>(ResultMessages.Task.UnauthorizedAccess);

                return Result.Success(new List<TaskResponse> { mapper.Map<TaskResponse>(task) }.AsEnumerable());
            }

            var query = context.Tasks
                .Where(t => t.UserId == userId)
                .AsQueryable();

            if (filterRequest.Status.Any())
            {
                query = query.Where(t => filterRequest.Status.Contains(t.Status));
            }
            if (filterRequest.TagIds.Any())
            {
                query = query.Where(t => filterRequest.TagIds.Contains(t.TagId));
            }
            if (filterRequest.Priorities.Any())
            {
                query = query.Where(t => filterRequest.Priorities.Contains(t.Priority));
            }
            var tasks = await query.ToListAsync(ct);

            var taskResponses = mapper.Map<IEnumerable<TaskResponse>>(tasks);

            return Result.Success(taskResponses);
        }

        private async Task<Task?> TaskByIdAsync(long taskId, CancellationToken ct = default)
        {
            return await context.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken: ct);
        }
    }
}


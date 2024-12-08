using AutoMapper;
using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Infrastructure.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Requests;
using Meowmentum.Server.Dotnet.Shared.Responses.Task;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.Extensions.Logging;
using Task = Meowmentum.Server.Dotnet.Core.Entities.Task;

namespace Meowmentum.Server.Dotnet.Business.Implementations;

public class TimeService(
    IRepository<TimeInterval> timeRepository,
    IRepository<Task> taskRepository,
    ITaskService taskService,
    IMapper mapper,
    ILogger<TimeService> logger) : ITimeService
{
    public async Task<Result<bool>> StartTimerAsync(long userId, long taskId, CancellationToken ct = default)
    {
        logger.LogInformation("Attempting to start timer for task with ID {TaskId} for user {UserId}", taskId, userId);

        var result = await taskRepository.GetFirstOrDefaultAsync(
            t => t.Id == taskId, ct);

        if (!result.IsSuccess)
        {
            logger.LogError("Task with ID {TaskId} not found ", taskId);
            return Result.Failure<bool>(ResultMessages.Task.TaskNotFound);
        }

        if (result.Data.UserId != userId)
        {
            logger.LogError("Task with ID {TaskId} belongs to other user ", taskId);
            return Result.Failure<bool>(ResultMessages.Task.UnauthorizedAccess);
        }

        var activeTimerResult = await timeRepository.GetFirstOrDefaultAsync(
        ti => ti.TaskId == taskId && ti.EndTime == null, ct);

        if (activeTimerResult.IsSuccess)
        {
            logger.LogWarning("An active timer already exists for task with ID {TaskId} for user {UserId}", taskId, userId);
            return Result.Failure<bool>(ResultMessages.Timer.ActiveTimerAlreadyExists);
        }

        TimeInterval timeInterval = new TimeInterval();
        timeInterval.TaskId = taskId;
        timeInterval.StartTime = DateTime.UtcNow;

        var addResult = await timeRepository.AddAsync(timeInterval, ct);

        if (addResult.IsSuccess)
        {
            logger.LogInformation("Successfully started timer for task with ID {TaskId} for user {UserId}", taskId, userId);
        }
        else
        {
            logger.LogError("Failed to start timer for task {TaskId} for user {UserId}: {ErrorMessage}", taskId, userId, addResult.ErrorMessage);
        }

        return addResult;
    }

    public async Task<Result<bool>> StopTimerAsync(long userId, long taskId, CancellationToken ct = default)
    {
        logger.LogInformation("Attempting to stop timer for task with ID {TaskId} for user {UserId}", taskId, userId);

        var result = await taskRepository.GetFirstOrDefaultAsync(
            t => t.Id == taskId, ct);

        if (!result.IsSuccess)
        {
            logger.LogError("Task with ID {TaskId} not found ", taskId);
            return Result.Failure<bool>(ResultMessages.Task.TaskNotFound);
        }

        if (result.Data.UserId != userId)
        {
            logger.LogError("Task with ID {TaskId} belongs to other user ", taskId);
            return Result.Failure<bool>(ResultMessages.Task.UnauthorizedAccess);
        }

        var lastIntervalResult = await timeRepository.GetFirstOrDefaultAsync(
            ti => ti.TaskId == taskId && ti.EndTime == null, ct);

        if (!lastIntervalResult.IsSuccess || lastIntervalResult.Data == null)
        {
            logger.LogError("No active timer found for task with ID {TaskId} for user {UserId}", taskId, userId);
            return Result.Failure<bool>(ResultMessages.Timer.NoActiveTimer);
        }

        var timeInterval = lastIntervalResult.Data;
        timeInterval.EndTime = DateTime.UtcNow;

        var updateResult = await timeRepository.UpdateAsync(timeInterval, ct);

        if (updateResult.IsSuccess)
        {
            logger.LogInformation("Successfully stopped timer for task with ID {TaskId} for user {UserId}", taskId, userId);
        }
        else
        {
            logger.LogError("Failed to stop timer for task {TaskId} for user {UserId}: {ErrorMessage}", taskId, userId, updateResult.ErrorMessage);
        }

        return updateResult;
    }

    public async Task<Result<bool>> UpdateTimerAsync(long timerId, long userId, TimerUpdateRequest updateRequest, CancellationToken ct = default)
    {
        logger.LogInformation("Attempting to update timer with ID {TimerId} for user {UserId}", timerId, userId);

        var timerResult = await timeRepository.GetFirstOrDefaultAsync(
            t => t.Id == timerId, ct);

        if (!timerResult.IsSuccess)
        {
            logger.LogError("Timer with ID {TimerId} not found", timerId);
            return Result.Failure<bool>(ResultMessages.Timer.TimerNotFound);
        }

        var timer = timerResult.Data;

        var taskResult = await taskRepository.GetFirstOrDefaultAsync(
            t => t.Id == timer.TaskId && t.UserId == userId, ct);

        if (!taskResult.IsSuccess)
        {
            logger.LogError("Timer with ID {TimerId} does not belong to user {UserId}", timerId, userId);
            return Result.Failure<bool>(ResultMessages.Timer.TimerNotBelongsToUser);
        }

        mapper.Map(updateRequest, timer);

        var updateResult = await timeRepository.UpdateAsync(timer, ct);

        if (updateResult.IsSuccess)
        {
            logger.LogInformation("Successfully updated timer with ID {TimerId} for user {UserId}", timerId, userId);
        }
        else
        {
            logger.LogError("Failed to update timer with ID {TimerId} for user {UserId}: {ErrorMessage}", timerId, userId, updateResult.ErrorMessage);
        }

        return updateResult;
    }

    public async Task<Result<IEnumerable<TimeInterval>>> GetTimersAsync(long userId, long? taskId, long? timeIntervalId, CancellationToken ct = default)
    {
        logger.LogInformation("GetTimersAsync called for userId: {UserId}, taskId: {TaskId}, timeIntervalId: {TimeIntervalId}",
            userId, taskId, timeIntervalId);

        if (timeIntervalId.HasValue)
        {
            logger.LogInformation("Fetching time interval with ID: {TimeIntervalId} for user {UserId}", timeIntervalId, userId);
            return await GetTimerAsync(userId, timeIntervalId.Value, ct);
        }

        if (taskId.HasValue)
        {
            logger.LogInformation("Fetching time intervals for task ID: {TaskId} for user {UserId}", taskId, userId);
            return await GetTimeIntervalsByTaskIdAsync(userId, taskId.Value, ct);
        }

        logger.LogInformation("Fetching all time intervals for user {UserId}", userId);
        return await GetAllTimeIntervalsForUserAsync(userId, ct);
    }

    public async Task<Result<bool>> DeleteTimerAsync(long userId, long timerId, CancellationToken ct = default)
    {
        logger.LogInformation("Attempting to delete time interval with ID {TimeIntervalId} for user {UserId}", timerId, userId);

        var timeIntervalResult = await timeRepository.GetFirstOrDefaultAsync(
            ti => ti.Id == timerId, ct);

        if (!timeIntervalResult.IsSuccess || timeIntervalResult.Data == null)
        {
            logger.LogError("Time interval with ID {TimeIntervalId} not found for user {UserId}", timerId, userId);
            return Result.Failure<bool>(ResultMessages.Timer.TimerNotFound);
        }

        var timer = timeIntervalResult.Data;

        var taskResult = await taskRepository.GetFirstOrDefaultAsync(
            t => t.Id == timer.TaskId && t.UserId == userId, ct);

        if (!taskResult.IsSuccess)
        {
            logger.LogError("Timer with ID {TimerId} does not belong to user {UserId}", timerId, userId);
            return Result.Failure<bool>(ResultMessages.Timer.TimerNotBelongsToUser);
        }

        var deleteResult = await timeRepository.DeleteAsync(timerId, ct);

        if (deleteResult.IsSuccess)
        {
            logger.LogInformation("Successfully deleted time interval with ID {TimeIntervalId} for user {UserId}", timerId, userId);
        }
        else
        {
            logger.LogError("Failed to delete time interval with ID {TimeIntervalId} for user {UserId}: {ErrorMessage}", timerId, userId, deleteResult.ErrorMessage);
        }

        return deleteResult;
    }

    private async Task<Result<IEnumerable<TimeInterval>>> GetTimerAsync(long userId, long timeIntervalId, CancellationToken ct)
    {
        var timeIntervalResult = await timeRepository.GetFirstOrDefaultAsync(
            ti => ti.Id == timeIntervalId, ct);

        if (!timeIntervalResult.IsSuccess)
        {
            logger.LogError("Failed to fetch time interval with ID: {TimeIntervalId} for user {UserId}. Error: {ErrorMessage}",
                timeIntervalId, userId, timeIntervalResult.ErrorMessage);
            return Result.Failure<IEnumerable<TimeInterval>>(timeIntervalResult.ErrorMessage);
        }

        var timer = timeIntervalResult.Data;

        var taskResult = await taskRepository.GetFirstOrDefaultAsync(
            t => t.Id == timer.TaskId && t.UserId == userId, ct);

        if (!taskResult.IsSuccess)
        {
            logger.LogError("Timer with ID {TimerId} does not belong to user {UserId}", timeIntervalId, userId);
            return Result.Failure<IEnumerable<TimeInterval>>(ResultMessages.Timer.TimerNotBelongsToUser);
        }

        logger.LogInformation("Successfully fetched time interval with ID: {TimeIntervalId} for user {UserId}", timeIntervalId, userId);
        return Result.Success(new List<TimeInterval> { timeIntervalResult.Data }.AsEnumerable());
    }

    private async Task<Result<IEnumerable<TimeInterval>>> GetTimeIntervalsByTaskIdAsync(long userId, long taskId, CancellationToken ct)
    {
        var taskResult = await taskService.GetTasksAsync(userId, new Shared.Requests.Task.TaskFilterRequest { TaskId = taskId }, ct);

        if (!taskResult.IsSuccess || taskResult.Data == null || !taskResult.Data.Any())
        {
            logger.LogError("No tasks found for user {UserId} with task ID {TaskId}", userId, taskId);
            return Result.Failure<IEnumerable<TimeInterval>>(ResultMessages.Task.TaskNotFound);
        }

        var task = taskResult.Data.First();

        return Result.Success((IEnumerable<TimeInterval>)task.TimeIntervals);
    }

    private async Task<Result<IEnumerable<TimeInterval>>> GetAllTimeIntervalsForUserAsync(long userId, CancellationToken ct)
    {
        var tasksResult = await taskService.GetTasksAsync(userId, new Shared.Requests.Task.TaskFilterRequest { }, ct);

        if (!tasksResult.IsSuccess)
        {
            logger.LogError("Failed to fetch tasks for user {UserId}. Error: {ErrorMessage}", userId, tasksResult.ErrorMessage);
            return Result.Failure<IEnumerable<TimeInterval>>(tasksResult.ErrorMessage);
        }

        var allTimeIntervals = tasksResult.Data.SelectMany(task => task.TimeIntervals);

        logger.LogInformation("Successfully fetched {Count} time intervals for user {UserId}", allTimeIntervals.Count(), userId);

        return Result.Success(allTimeIntervals);
    }
}



using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Infrastructure.Abstractions;
using Meowmentum.Server.Dotnet.Persistence.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Extensions;
using Meowmentum.Server.Dotnet.Shared.Options.Redis;
using Meowmentum.Server.Dotnet.Shared.Requests.Email;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Task = Meowmentum.Server.Dotnet.Core.Entities.Task;
using TaskStatus = Meowmentum.Server.Dotnet.Core.Entities.TaskStatus;

namespace Meowmentum.Server.Dotnet.Business.Implementations;

public class TaskNotificationService(
    UserManager<AppUser> userManager,
    IRedisCacheService redisCacheService,
    IOptions<OverdueTaskDbConfig> overdueTaskOptions,
    IOptions<UpcomingTaskDbConfig> upcomingTaskOptions,
    IRepository<Task> taskRepository, 
    IEmailService emailService, 
    ILogger<INotificationService> logger) : INotificationService
{
    private readonly UpcomingTaskDbConfig _upcomingTaskConfig = upcomingTaskOptions.Value;
    private readonly TimeSpan _upcomingTaskExpirationTime = TimeSpan.FromMinutes(upcomingTaskOptions.Value.ExpirationTimeInMinutes);

    private readonly OverdueTaskDbConfig _overdueTaskConfig = overdueTaskOptions.Value;
    private readonly TimeSpan _overdueTaskExpirationTime = TimeSpan.FromMinutes(overdueTaskOptions.Value.ExpirationTimeInMinutes);

    public async Task<Result<bool>> NotifyAboutUpcomingTasksAsync(CancellationToken ct = default)
    {
        try
        {
            var result = await taskRepository.GetAllAsync(
                t => t.Deadline.HasValue &&
                    ((DateTimeOffset)t.Deadline.Value) >= DateTimeOffset.UtcNow &&
                    ((DateTimeOffset)t.Deadline.Value) <= DateTimeOffset.UtcNow.AddDays(1) &&
                    t.Status != TaskStatus.Completed,
                ct: ct);

            if (!result.IsSuccess)
            {
                logger.LogError($"Failed to retrieve tasks. Error: {result.ErrorMessage}");
                return Result.Failure<bool>(ResultMessages.Task.FailToGetTask);
            }

            var tasks = result.Data;

            foreach (var task in tasks)
            {
                var redisKey = _upcomingTaskConfig.Prefix.Append(task.Id.ToString());

                var taskExists = await redisCacheService.ExistsAsync(redisKey, _upcomingTaskConfig.DbNumber, ct);
                if (taskExists.IsSuccess)
                {
                    logger.LogInformation($"Notification already sent for upcoming task with id: {task.Id}, skipping");
                    continue;
                }

                var user = await userManager.FindByIdAsync(task.UserId.ToString());
                if (user is null)
                {
                    logger.LogError($"User not found with id: {task.UserId}");
                    return Result.Failure<bool>(ResultMessages.User.UserNotFound);
                }

                if (user.Email is not null)
                {
                    var taskUrl = GenerateTaskUrl(task.Id);
                    var newRequest = new UpcomingTaskSendingRequest 
                    { 
                        Email = user.Email, 
                        UserName = user.UserName ?? "User",
                        TaskName = task.Title,
                        TaskUrl = taskUrl
                    };
                    var emailResult = await emailService.SendUpcomingTaskNotificationByEmailAsync(newRequest, ct);

                    if (!emailResult.IsSuccess)
                    {
                        logger.LogError($"Failed to send email for task with id: {task.Id} to user {task.UserId}. " +
                            $"Error: {emailResult.ErrorMessage}");
                        return Result.Failure<bool>($"{ResultMessages.Email.FailToSend} for upcoming task {task.Id}");
                    }

                    var setResult = await redisCacheService.SetAsync(
                        redisKey,
                        "sent",
                        _upcomingTaskExpirationTime,
                        _upcomingTaskConfig.DbNumber,
                        true,
                        ct);
                    if (!setResult.IsSuccess)
                    {
                        logger.LogError($"Failed to set upcoming task with id: {task.Id} for user with id: {task.UserId} in redis cache");
                        return Result.Failure<bool>(setResult.ErrorMessage);
                    }

                    logger.LogInformation($"Notification sent for upcoming task with id: {task.Id}");
                }
            }

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while notifying about upcoming tasks");
            return Result.Failure<bool>(ResultMessages.Email.FailToSend);
        }
    }

    private string GenerateTaskUrl(long taskId)
    {
        return $"http://127.0.0.1:8080/tasks/{taskId}"; // todo: change to real url
    }

    public async Task<Result<bool>> NotifyAboutOverdueTasksAsync(CancellationToken ct = default)
    {
        try
        {
            var result = await taskRepository.GetAllAsync(
                t => t.Deadline.HasValue &&
                    ((DateTimeOffset)t.Deadline.Value) < DateTimeOffset.UtcNow &&
                    t.Status != TaskStatus.Completed,
                ct: ct);

            if (!result.IsSuccess)
            {
                logger.LogError($"Failed to retrieve tasks. Error: {result.ErrorMessage}");
                return Result.Failure<bool>(ResultMessages.Task.FailToGetTask);
            }

            var tasks = result.Data;
            var tasksByUser = tasks
                .GroupBy(t => t.UserId)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var userTasks in tasksByUser)
            {
                var userId = userTasks.Key;
                var tasksForUser = userTasks.Value;

                var redisKey = _overdueTaskConfig.Prefix.Append($"user:{userId}:overdueCount");

                var taskExists = await redisCacheService.ExistsAsync(redisKey, _overdueTaskConfig.DbNumber, ct);
                var currentOverdueCount = 0;

                if (taskExists.IsSuccess && taskExists.Data)
                {
                    var currentOverdueCountResult = await redisCacheService.GetAsync<int>(redisKey, _overdueTaskConfig.DbNumber, ct);
                    if (currentOverdueCountResult.IsSuccess)
                    {
                        currentOverdueCount = currentOverdueCountResult.Data;
                    }
                    else
                    {
                        logger.LogError($"Failed to retrieve overdue count for user with id: {userId} from Redis");
                        return Result.Failure<bool>(currentOverdueCountResult.ErrorMessage);
                    }
                }

                if (currentOverdueCount != tasksForUser.Count || !taskExists.Data)
                {
                    var setCountResult = await redisCacheService.SetAsync(
                        redisKey,
                        tasksForUser.Count,
                        _overdueTaskExpirationTime,
                        _overdueTaskConfig.DbNumber,
                        true,
                        ct);

                    if (!setCountResult.IsSuccess)
                    {
                        logger.LogError($"Failed to update overdue count for user with id: {userId} in Redis");
                        return Result.Failure<bool>(setCountResult.ErrorMessage);
                    }

                    logger.LogInformation($"Updated overdue task count for user {userId} in Redis");
                }
                else
                {
                    logger.LogInformation($"No new overdue tasks for user with id: {userId}, skipping notification");
                    continue;
                }

                var user = await userManager.FindByIdAsync(userId.ToString());
                if (user is null)
                {
                    logger.LogError($"User not found with id: {userId}");
                    return Result.Failure<bool>(ResultMessages.User.UserNotFound);
                }

                if (user.Email is not null)
                {
                    var newRequest = new OverdueTaskSendingRequest
                    {
                        Email = user.Email,
                        UserName = user.UserName ?? "User",
                        TaskCount = tasksForUser.Count
                    };

                    var emailResult = await emailService.SendOverdueTaskNotificationByEmailAsync(newRequest, ct);

                    if (!emailResult.IsSuccess)
                    {
                        logger.LogError($"Failed to send email for overdue tasks to user with id: {userId}. " +
                            $"Error: {emailResult.ErrorMessage}");
                        return Result.Failure<bool>($"{ResultMessages.Email.FailToSend} for overdue tasks");
                    }

                    logger.LogInformation($"Notification sent for overdue tasks to user with id: {userId}");
                }
            }

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while notifying about overdue tasks");
            return Result.Failure<bool>("An error occurred while notifying about overdue tasks");
        }
    }
}

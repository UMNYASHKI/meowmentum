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
                    logger.LogInformation($"Notification already sent for upcoming task {task.Title}, skipping");
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
                    var message = $"Task {task.Title} is due tomorrow!";
                    var newRequest = new NotificationSendingRequest { Email = user.Email, Message = message };
                    var emailResult = await emailService.SendNotificationForUserEmailAsync(newRequest, ct);

                    if (!emailResult.IsSuccess)
                    {
                        logger.LogError($"Failed to send email for task {task.Title} to user {task.UserId}. " +
                            $"Error: {emailResult.ErrorMessage}");
                        return Result.Failure<bool>($"{ResultMessages.Email.FailToSend} for upcoming task {task.Title}");
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
                        logger.LogError($"Failed to set upcoming task {task.Id} for user {task.UserId} in redis cache");
                        return Result.Failure<bool>(setResult.ErrorMessage);
                    }

                    logger.LogInformation($"Notification sent for upcoming task {task.Title}.");
                }
            }

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while notifying about upcoming tasks.");
            return Result.Failure<bool>(ResultMessages.Email.FailToSend);
        }
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

            foreach (var task in tasks)
            {
                var redisKey = _overdueTaskConfig.Prefix.Append(task.Id.ToString());

                var taskExists = await redisCacheService.ExistsAsync(redisKey, _overdueTaskConfig.DbNumber, ct);
                if (taskExists.IsSuccess)
                {
                    logger.LogInformation($"Notification already sent for overdue task {task.Title}, skipping");
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
                    var message = $"Task {task.Title} is overdue! Please complete it as soon as possible";
                    var newRequest = new NotificationSendingRequest { Email = user.Email, Message = message };
                    var emailResult = await emailService.SendNotificationForUserEmailAsync(newRequest, ct);

                    if (!emailResult.IsSuccess)
                    {
                        logger.LogError($"Failed to send email for overdue task {task.Title} to user {task.UserId}. Error: {emailResult.ErrorMessage}");
                        return Result.Failure<bool>($"{ResultMessages.Email.FailToSend} for overdue task {task.Title}");
                    }

                    var setResult = await redisCacheService.SetAsync(
                        redisKey,
                        "sent",
                        _overdueTaskExpirationTime,
                        _overdueTaskConfig.DbNumber, 
                        true, 
                        ct);
                    if (!setResult.IsSuccess)
                    {
                        logger.LogError($"Failed to set overdue task {task.Id} for user {task.UserId} in redis cache");
                        return Result.Failure<bool>(setResult.ErrorMessage);
                    }

                    logger.LogInformation($"Notification sent for overdue task {task.Title}.");
                }
            }

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while notifying about overdue tasks.");
            return Result.Failure<bool>("An error occurred while notifying about overdue tasks.");
        }
    }
}

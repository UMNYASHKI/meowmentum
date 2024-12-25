using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Infrastructure.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Requests.Email;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Task = Meowmentum.Server.Dotnet.Core.Entities.Task;
using TaskStatus = Meowmentum.Server.Dotnet.Core.Entities.TaskStatus;

namespace Meowmentum.Server.Dotnet.Business.Implementations;

public class TaskNotificationService(
    UserManager<AppUser> userManager,
    IRepository<Task> taskRepository, 
    IEmailService emailService, 
    ILogger<INotificationService> logger) : INotificationService
{
    public async Task<Result<bool>> NotifyAboutUpcomingTasksAsync(CancellationToken ct = default)
    {
        try
        {
            var result = await taskRepository.GetAllAsync(
                t => t.Deadline.HasValue &&
                    ((DateTimeOffset)t.Deadline.Value).Date == DateTimeOffset.UtcNow.AddDays(1).Date && 
                    t.Status != TaskStatus.Completed,
                ct: ct);

            if (!result.IsSuccess)
            {
                logger.LogError($"Failed to retrieve tasks. Error: {result.ErrorMessage}");
                return Result.Failure<bool>("Failed to retrieve tasks");
            }

            var tasks = result.Data;

            foreach (var task in tasks)
            {
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
                        logger.LogError($"Failed to send email for task {task.Title} to user {task.UserId}. Error: {emailResult.ErrorMessage}");
                        return Result.Failure<bool>($"Failed to send email for task {task.Title}");
                    }
                }
            }

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while notifying about upcoming tasks.");
            return Result.Failure<bool>("An error occurred while notifying about upcoming tasks.");
        }
    }

    public async Task<Result<bool>> NotifyAboutOverdueTasksAsync(CancellationToken ct = default)
    {
        try
        {
            var result = await taskRepository.GetAllAsync(
                t => t.Deadline.HasValue &&
                    ((DateTimeOffset)t.Deadline.Value).Date < DateTimeOffset.UtcNow.Date && 
                    t.Status != TaskStatus.Completed,
                ct: ct);

            if (!result.IsSuccess)
            {
                logger.LogError($"Failed to retrieve tasks. Error: {result.ErrorMessage}");
                return Result.Failure<bool>("Failed to retrieve tasks");
            }

            var tasks = result.Data;

            foreach (var task in tasks)
            {
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
                        return Result.Failure<bool>($"Failed to send email for overdue task {task.Title}");
                    }
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

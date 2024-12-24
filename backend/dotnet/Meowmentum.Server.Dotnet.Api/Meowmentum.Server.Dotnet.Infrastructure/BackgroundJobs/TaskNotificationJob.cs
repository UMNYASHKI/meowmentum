using Meowmentum.Server.Dotnet.Business.Abstractions;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Meowmentum.Server.Dotnet.Infrastructure.BackgroundJobs;

public class TaskNotificationJob(
    INotificationService notificationService, 
    ILogger<TaskNotificationJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            logger.LogInformation("Executing TaskNotificationJob...");

            await notificationService.NotifyAboutUpcomingTasksAsync(context.CancellationToken);

            await notificationService.NotifyAboutOverdueTasksAsync(context.CancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing TaskNotificationJob.");
        }
    }
}


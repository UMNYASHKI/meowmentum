using Meowmentum.Server.Dotnet.Infrastructure.BackgroundJobs;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Meowmentum.Server.Dotnet.Infrastructure.Extensions;

public static class QuartzConfiguration
{
    public static IServiceCollection AddQuartzServices(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            q.AddJob<TaskNotificationJob>(opts => opts.WithIdentity("TaskNotificationJob"))
             .AddTrigger(opts => opts.ForJob("TaskNotificationJob")
                                      .WithIdentity("TaskNotificationTrigger")
                                      .StartNow()
                                      .WithSimpleSchedule(x => x.WithIntervalInMinutes(5).RepeatForever()));
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        return services;
    }
}


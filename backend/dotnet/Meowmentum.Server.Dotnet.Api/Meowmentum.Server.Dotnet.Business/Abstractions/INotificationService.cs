using Meowmentum.Server.Dotnet.Shared.Results;

namespace Meowmentum.Server.Dotnet.Business.Abstractions;

public interface INotificationService
{
    Task<Result<bool>> NotifyAboutUpcomingTasksAsync(CancellationToken ct = default);
    Task<Result<bool>> NotifyAboutOverdueTasksAsync(CancellationToken ct = default);
}

using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Business.Implementations;
using Meowmentum.Server.Dotnet.Infrastructure.Abstractions;
using Meowmentum.Server.Dotnet.Infrastructure.HelperServices;
using Meowmentum.Server.Dotnet.Shared.Options.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Meowmentum.Server.Dotnet.Infrastructure.Extensions;

public static class HelperServicesConfiguration
{
    public static IServiceCollection AddHelperServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OtpDbConfig>(configuration.GetSection(OtpDbConfig.SectionName));
        services.AddScoped<IOtpManager, OtpManager>();

        services.Configure<TokenBlacklistDbConfig>(configuration.GetSection(TokenBlacklistDbConfig.SectionName));
        services.AddScoped<ITokenBlackListManager, TokenBlackListManager>();

        services.Configure<UpcomingTaskDbConfig>(configuration.GetSection(OverdueTaskDbConfig.SectionName));
        services.Configure<OverdueTaskDbConfig>(configuration.GetSection(OverdueTaskDbConfig.SectionName));
        services.AddScoped<INotificationService, TaskNotificationService>();

        return services;
    }
}
